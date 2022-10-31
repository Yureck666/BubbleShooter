using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[RequireComponent(typeof(Grid))]
public class GridBalls : MonoBehaviour
{
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private float ballsDestroyInterval;
    [SerializeField] private int ballsToDestroy;
    [SerializeField] private GameUi gameUi;

    [Space, Header("Random")]
    [SerializeField] private Ball ballPrefab;
    [SerializeField] private float emptyChance;
    [SerializeField] private int rowsCount;
    [SerializeField] private ColorsProvider colorProvider;

    public UnityEvent onPlaceBallFinish { get; private set; }

    private Grid _grid;
    private Ball[,] _awesomeBalls;

    private Grid Grid
    {
        get
        {
            if (_grid == null)
                _grid = GetComponent<Grid>();

            return _grid;
        }
    }
    
    public void Init()
    {
        onPlaceBallFinish = new UnityEvent();
        _grid = GetComponent<Grid>();
        CreateRandomBalls();
        Fit();
    }

    private void CreateRandomBalls()
    {
        for (var i = 0; i < rowsCount * gridWidth; i++)
        {
            var spawnBall = Random.Range(0f, 1f) > emptyChance;
            if (spawnBall)
            {
                var ball = Instantiate(ballPrefab);
                ball.transform.SetParent(transform);
                ball.SetColor(colorProvider.GetRandom().ColorName);
            }
            else
            {
                Instantiate(new GameObject()).transform.SetParent(transform);
            }
        }
    }

    [CanBeNull]
    public Ball GetBall(Vector3Int position)
    {
        if (position.x < 0 || position.x >= gridWidth ||
            position.y < 0 || position.y >= gridHeight)
            return default;

        return _awesomeBalls[position.x, position.y];
    }

    public Vector3Int[] GetNeighbors(Vector3Int position)
    {
        var isRawEven = position.y % 2 != 0;
        var offsets = new Vector3Int[]
        {
            Vector3Int.up,
            Vector3Int.down,
            Vector3Int.right,
            Vector3Int.left,
            isRawEven ? Vector3Int.down + Vector3Int.right : Vector3Int.down + Vector3Int.left,
            isRawEven ? Vector3Int.up + Vector3Int.right : Vector3Int.up + Vector3Int.left
        };


        return offsets.Select(offset => position + offset)
            .Where(offset =>
                !(offset.x < 0
                  || offset.x >= gridWidth
                  || offset.y < 0
                  || offset.y >= gridHeight)).ToArray();
    }

    [ContextMenu("Fit")]
    private void Fit()
    {
        var gridPosition = new Vector3Int(0, 0);

        _awesomeBalls = new Ball[gridWidth, gridHeight];
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out Ball ball))
            {
                child.position = Grid.GetCellCenterWorld(gridPosition);
                child.gameObject.name = $"Ball {gridPosition}";
                _awesomeBalls[gridPosition.x, gridPosition.y] = ball;
                ball.Init();
                ball.Place();
                ball.SetGridPosition(gridPosition);
            }
            gridPosition.x += 1;
            if (gridPosition.x > gridWidth - 1)
            {
                gridPosition.x = 0;
                gridPosition.y += 1;
            }
        }

        FallAllFreeBalls(false);
    }

    public void PlaceBall(Ball flyBall, Ball collisionBall)
    {
        Debug.DrawRay(collisionBall.transform.position, Vector3.back * 3, Color.white, 2);
        

        var position = GetFreePositions(GetNeighbors(collisionBall.GridPosition))
            .OrderBy(pos => Vector3.Distance(Grid.GetCellCenterWorld(pos), flyBall.transform.position))
            .First();
        _awesomeBalls[position.x, position.y] = flyBall;
        flyBall.Place();
        flyBall.SetGridPosition(position);
        flyBall.transform.SetParent(transform);
        flyBall.MagnetToPositionLocal(Grid.GetCellCenterLocal(position));
        flyBall.gameObject.name = $"Ball {position}";
        
        var properBalls = CheckNeighborsColor(flyBall);
        Debug.Log(properBalls.Length);
        //EditorApplication.isPaused = true;
        if (properBalls.Length >= ballsToDestroy)
            DestroyBalls(properBalls);
        
        FallAllFreeBalls(true);

        if (IsGridEmpty())
        {
            gameUi.SetWinPanelActive(true);
        }
        
        onPlaceBallFinish.Invoke();
    }

    private Vector3Int[] GetFreePositions(Vector3Int[] positions)
    {
        return positions.Where(pos => GetBall(pos) == default).ToArray();
    }

    private void FallAllFreeBalls(bool animated)
    {
        var saveBalls = new List<Ball>();
        var gridPosition = new Vector3Int(0, 0);
        for (var x = 0; x < gridWidth; x++)
        {
            gridPosition.x = x;
            var ball = GetBall(gridPosition);
            if (ball != null)
            {
                AddAllConnectedBallsRecursively(ball, saveBalls);
            }
        }

        foreach (var ball in _awesomeBalls)
        {
            if (!saveBalls.Contains(ball) && ball != null)
            {
                _awesomeBalls[ball.GridPosition.x, ball.GridPosition.y] = null;
                if (animated)
                    ball.FallDestroy();
                else 
                    Destroy(ball.gameObject);
            }
        }
    }

    private void AddAllConnectedBallsRecursively(Ball ball, List<Ball> balls)
    {
        if (!balls.Contains(ball)) 
            balls.Add(ball);
        var neighbors = GetNeighbors(ball.GridPosition)
            .Where(neighbor =>
            {
                var neighborBall = GetBall(neighbor);
                return !balls.Contains(neighborBall) &&
                       neighborBall != null;
            })
            .ToArray();
        balls.AddRange(neighbors.Select(GetBall));
        foreach (var neighbor in neighbors)
        {
            AddAllConnectedBallsRecursively(GetBall(neighbor), balls);
        }
    }

    private bool IsGridEmpty()
    {
        return _awesomeBalls.Cast<Ball>().All(ball => ball == null);
    }

    private Ball[] CheckNeighborsColor(Ball ball, List<Ball> balls = null)
    {
        balls ??= new List<Ball>();
        if (!balls.Contains(ball)) 
            balls.Add(ball);
        var neighbors = GetNeighbors(ball.GridPosition)
            .Where(neighbor =>
            {
                var neighborBall = GetBall(neighbor);
                return !balls.Contains(neighborBall) &&
                       neighborBall != null &&
                       neighborBall.GetColor() == ball.GetColor();
            })
            .ToArray();
        balls.AddRange(neighbors.Select(GetBall));
        foreach (var neighbor in neighbors)
        {
            CheckNeighborsColor(GetBall(neighbor), balls);
        }

        return balls.ToArray();
    }

    private void DestroyBalls(Ball[] balls)
    {
        var index = 0;
        foreach (var ball in balls)
        {
            var gridPosition = ball.GridPosition;
            _awesomeBalls[gridPosition.x, gridPosition.y] = null;
            ball.ScaleDestroy(index*ballsDestroyInterval);
            index++;
        }
    }
}