using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallShooter : MonoBehaviour
{
    [SerializeField] private GridBalls gridBalls;
    [SerializeField] private InputCatcher inputCatcher;
    [SerializeField] private BallsStack ballsStack;
    [SerializeField] private LayerMask shootMask;

    private Camera _mainCamera;
    private Vector2 _touchPosition;
    private bool _readyToShoot;

    private void Awake()
    {
        _readyToShoot = true;
        _mainCamera = Camera.main;
        
        inputCatcher.Init();
        inputCatcher.PointerDownEvent.AddListener(SetTouchPosition);
        inputCatcher.DragEvent.AddListener(SetTouchPosition);
        inputCatcher.PointerUpEvent.AddListener(ShootBall);
        
        gridBalls.Init();
        gridBalls.onPlaceBallFinish.AddListener(() => _readyToShoot = true);
    }

    private void SetTouchPosition(Vector2 point)
    {
        _touchPosition = point;
    }

    private void ShootBall()
    {
        if (ballsStack.IsBusyForAnimation || !_readyToShoot) return;
        if (Physics.Raycast(_mainCamera.ScreenPointToRay(_touchPosition), out var hit, float.MaxValue, shootMask, QueryTriggerInteraction.Collide))
        {
            var ball = ballsStack.GetBall();
            ball.ShootInDirection(hit.point);
            ball.OnBallCollision.AddListener(collisionBall => gridBalls.PlaceBall(ball, collisionBall));
            ball.OnDestroyAction.AddListener(() => _readyToShoot = true);
            _readyToShoot = false;
        }
    }
}
