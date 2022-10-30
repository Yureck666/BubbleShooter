using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private Camera camera;
    [SerializeField] private Vector3 moveSpeed;
    [SerializeField] private float maxYValue;

    private List<Ball> _ballsInside;

    private void Awake()
    {
        _ballsInside = new List<Ball>();
    }

    private void Update()
    {
        if (camera.transform.position.y > maxYValue || _ballsInside.Any(ball => ball.IsInGrid && ball))
        {
            return;
        }

        camera.transform.position += moveSpeed * Time.deltaTime;
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
