using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Grid))]
public class GridBalls : MonoBehaviour
{
    [SerializeField] private int gridWidth;
    [SerializeField] private int gridHeight;
    [SerializeField] private float ballsDestroyInterval;

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

    private void Awake()
    {
        onPlaceBallFinish = new UnityEvent();
        _grid = GetComponent<Grid>();
        Fit();
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
        FallAllFreeBalls();
    }

    public void PlaceBall(Ball flyBall, Ball collisionBall)
    {
        Debug.DrawRay(collisionBall.transform.position, Vector3.back * 3, Color.white, 2);
        foreach (var neighbor in GetNeighbors(collisionBall.GridPosition))
        {
            Debug.DrawRay(Grid.GetCellCenterWorld(neighbor), Vector3.back * 3, Color.black, 2);
        }

        var position = GetFreePositions(GetNeighbors(collisionBall.GridPosition))
            .OrderBy(pos => Vector3.Distance(Grid.GetCellCenterWorld(pos), flyBall.transform.position))
            .First();
        _awesomeBalls[position.x, position.y] = flyBall;
        flyBall.Place();
        flyBall.SetGridPosition(position);
        flyBall.transform.SetParent(transform);
        flyBall.LocalMoveToPositionAnimated(Grid.GetCellCenterLocal(position));
        
        var properBalls = CheckNeighborsColor(flyBall);
        if (properBalls.Length >= 3)
            DestroyBalls(properBalls);
        
        FallAllFreeBalls();
        
        onPlaceBallFinish.Invoke();
    }

    private Vector3Int[] GetFreePositions(Vector3Int[] positions)
    {
        return positions.Where(pos => GetBall(pos) == default).ToArray();
    }

    private void FallAllFreeBalls()
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
                ball.FallDestroy();
        }
    }

    private void AddAllConnectedBallsRecursively(Ball ball, List<Ball> balls)
    {
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

    private Ball[] CheckNeighborsColor(Ball ball, List<Ball> balls = null)
    {
        balls ??= new List<Ball>();
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