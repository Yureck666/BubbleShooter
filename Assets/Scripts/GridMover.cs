using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridMover : MonoBehaviour
{
    [SerializeField] private GridBalls grid;
    [SerializeField] private Vector3 moveSpeed;
    [SerializeField] private float minYValue;

    private List<Ball> _ballsInside;

    private void Awake()
    {
        _ballsInside = new List<Ball>();
    }

    private void Update()
    {
        if (grid.transform.position.y < minYValue || _ballsInside.Any(ball => ball.IsInGrid && ball))
        {
            return;
        }

        grid.transform.position += moveSpeed * Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Ball ball))
        {
            _ballsInside.Add(ball);
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Ball ball))
        {
            _ballsInside.Remove(ball);
        }
    }
}
