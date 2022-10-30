using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Border : MonoBehaviour
{
    private enum BorderBehaviour
    {
        Reflect,
        Destroy
    }

    [SerializeField] private BorderBehaviour behaviour;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Ball ball))
        {
            if (behaviour == BorderBehaviour.Reflect) 
                ball.Reflect(transform.forward);
            else if (behaviour == BorderBehaviour.Destroy)
                ball.ScaleDestroy();
        }
    }
}
