using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;

public class BallsStack : MonoBehaviour
{
    [Serializable]
    private class BallPosition
    {
        public Transform position;
        public float startDelay;
        public Ball BallInPosition { get; private set; }

        public void SetBallInPosition(Ball ball)
        {
            BallInPosition = ball;
        }
    }

    [SerializeField] private BallCreator ballCreator;
    [SerializeField] private Transform ballsParent;
    [SerializeField] private BallPosition shootPosition;
    [SerializeField] private BallPosition reservePosition;

    public bool IsBusyForAnimation { get; private set; }

    public Ball GetBall()
    {
        var ball = shootPosition.BallInPosition;
        MoveReserve();
        return ball;
    }

    private void MoveReserve()
    {
        var ball = reservePosition.BallInPosition;
        IsBusyForAnimation = true;
        ball.MoveToPositionAnimated(shootPosition.position.position, () =>
        {
            IsBusyForAnimation = false;
        });
        shootPosition.SetBallInPosition(ball);
        AddReserveBall();
    }

    private void Awake()
    {
        AddBallByScale(shootPosition, true);
        AddReserveBall();
    }

    private Ball AddReserveBall()
    {
        var reserveBall = AddBallByScale(reservePosition, true);
        reserveBall.SetColliderActive(false);
        reserveBall.SetInReserve(true);
        return reserveBall;
    }

    private Ball AddBallByScale(BallPosition ballPosition, bool delayed = false)
    {
        var ball = ballCreator.CreateBall();
        ball.transform.SetParent(ballsParent);
        ball.transform.position = ballPosition.position.position;
        ballPosition.SetBallInPosition(ball);
        ball.ShowAnimated(delayed ? ballPosition.startDelay : 0);
        return ball;
    }
}