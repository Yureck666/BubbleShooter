using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class Ball : MonoBehaviour
{
    [SerializeField] private ColorsProvider colorsProvider;
    [SerializeField] private ColorsProvider.ColorEnum colorName = ColorsProvider.ColorEnum.Red;
    [SerializeField] private float startScaleTime;
    [SerializeField] private Ease scaleEase;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Ease moveEase;
    [SerializeField] private float magnetSpeed;
    [SerializeField] private Ease magnetEase;
    [SerializeField] private float shootSpeed;
    [SerializeField] private float dieScaleTime;
    [SerializeField] private Ease dieEase;
    [SerializeField] private Vector3 dieForce;
    [SerializeField] private float dieDelay;

    public UnityEvent<Ball> OnBallCollision { get; private set; }
    public UnityEvent OnDestroyAction { get; private set; }
    public Vector3Int GridPosition { get; private set; }
    public bool IsInGrid { get; private set; }
    public bool IsInReserve { get; private set; }

    private Renderer _renderer;
    private Tween _scaleTween;
    private Tween _moveTween;
    private Rigidbody _rigidbody;
    private Collider _collider;
    private bool _isInFly;

    private Renderer Renderer
    {
        get
        {
            if (_renderer == null)
            {
                _renderer = GetComponent<Renderer>();
            }

            return _renderer;
        }
    }

    public void SetGridPosition(Vector3Int position)
    {
        GridPosition = position;
    }

    public void Init()
    {
        OnDestroyAction = new UnityEvent();
        OnBallCollision = new UnityEvent<Ball>();
        
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.isKinematic = true;
        _rigidbody.useGravity = false;
        
        _renderer = GetComponent<Renderer>();
        _renderer.sharedMaterial = GetMaterial();

        _collider = GetComponent<Collider>();
    }

    public void SetInReserve(bool active)
    {
        IsInReserve = active;
    }

    public ColorsProvider.ColorEnum GetColor()
    {
        return colorName;
    }

    private Material GetMaterial()
    {
        return colorsProvider.GetByName(colorName).Material;
    }

    public void SetColliderActive(bool active)
    {
        _collider.enabled = active;
    } 

    public void SetColor(ColorsProvider.ColorEnum color)
    {
        colorName = color;
    }

    public void ShowAnimated(float delay = 0)
    {
        var scale = transform.localScale;
        transform.localScale = Vector3.zero;
        KillScaleTween();
        transform.DOScale(scale, startScaleTime).SetEase(scaleEase).SetDelay(delay);
    }

    public void MoveToPositionAnimated(Vector3 position, Action onEndAction = null)
    {
        KillMoveTween();
        _moveTween = transform.DOMove(position, GetMoveSpeed(position, moveSpeed)).SetEase(moveEase).OnComplete(() => onEndAction?.Invoke());
    }

    public void MagnetToPositionLocal(Vector3 position, Action onEndAction = null)
    {
        KillMoveTween();
        _moveTween = transform.DOLocalMove(position, GetMoveSpeed(position, magnetSpeed)).SetEase(magnetEase).OnComplete(() => onEndAction?.Invoke());
    }

    public void ShootInDirection(Vector3 direction)
    {
        _isInFly = true;
        SetColliderActive(true);
        _rigidbody.isKinematic = false;
        direction = (direction - transform.position).normalized;
        _rigidbody.AddForce(direction*shootSpeed, ForceMode.Impulse);
    }

    public void Reflect(Vector3 normal)
    {
        _rigidbody.velocity = Vector3.Reflect(_rigidbody.velocity, normal);
    }

    public void ScaleDestroy(float delay = 0)
    {
        KillScaleTween();
        _scaleTween = transform.DOScale(Vector3.zero, dieScaleTime).SetEase(dieEase).SetDelay(delay).OnComplete(() => Destroy(gameObject));
    }

    public void FallDestroy()
    {
        KillMoveTween();
        IsInGrid = false;
        _rigidbody.useGravity = true;
        _rigidbody.isKinematic = false;
        SetColliderActive(false);
        _rigidbody.AddForce(dieForce, ForceMode.Impulse);
        StartCoroutine(DestroyDelayed());
    }

    private IEnumerator DestroyDelayed()
    {
        yield return new WaitForSeconds(dieDelay);
        Destroy(gameObject);
    }

    public void Place()
    {
        _rigidbody.isKinematic = true;
        _isInFly = false;
        IsInGrid = true;
    }

    private float GetMoveSpeed(Vector3 position, float speed)
    {
        return Vector3.Distance(position, transform.position) / speed;
    }

    private void KillScaleTween()
    {
        if (_scaleTween == null) return;
        _scaleTween.Kill();
        _scaleTween = null;
    }

    private void KillMoveTween()
    {
        if (_moveTween == null) return;
        _moveTween.Kill();
        _moveTween = null;
    }

    private void OnValidate()
    {
        Renderer.sharedMaterial = GetMaterial();
    }

    private void OnDestroy()
    {
        OnDestroyAction.Invoke();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.TryGetComponent(out Ball collisionBall) && _isInFly)
        {
            OnBallCollision.Invoke(collisionBall);
        }
    }
}
