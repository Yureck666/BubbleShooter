using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class CameraAspectRatioFitter : MonoBehaviour
{
    [SerializeField, Range(1f,10f)] private float multiply;
    private Camera _camera;

    private Camera Camera
    {
        get
        {
            if (_camera == null)
                _camera = GetComponent<Camera>();

            return _camera;
        }
    }
    
    private void Awake()
    {
        Camera.orthographicSize = GetOrthographicSize();        
    }

#if UNITY_EDITOR
    private void Update()
    {
        Camera.orthographicSize = GetOrthographicSize();
    }
#endif

    private float GetOrthographicSize() => ((float)Screen.height / Screen.width) * multiply;
}
