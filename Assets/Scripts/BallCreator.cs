using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class BallCreator : MonoBehaviour
{
    [SerializeField] private bool isRandom;
    [SerializeField] private ColorsProvider colorsProvider;
    [SerializeField] private ColorsProvider.ColorEnum[] ballsOrder;
    [SerializeField] private Ball ballPrefab;

    private int _ballIndex;

    public Ball CreateBall()
    {
        return isRandom ? CreateBallRandom() : CreateBallByOrder();
    }
    
    private Ball CreateBallByOrder()
    {
        var ball = Instantiate(ballPrefab);
        ball.SetColor(ballsOrder[_ballIndex]);
        ball.Init();
        _ballIndex = _ballIndex == ballsOrder.Length-1 ? 0 : _ballIndex + 1;
        return ball;
    }

    private Ball CreateBallRandom()
    {
        var ball = Instantiate(ballPrefab);
        ball.SetColor(colorsProvider.GetRandom().ColorName);
        ball.Init();
        return ball;
    }
}