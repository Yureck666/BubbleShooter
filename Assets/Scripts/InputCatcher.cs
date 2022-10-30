using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class InputCatcher : MonoBehaviour, IPointerDownHandler, IDragHandler, IBeginDragHandler, IEndDragHandler,
    IPointerUpHandler

{
    public  UnityEvent<Vector2> PointerDownEvent { get; private set; }
    public  UnityEvent PointerUpEvent { get; private set; }
    public  UnityEvent<Vector2> DragEvent { get; private set; }

    public void Init()
    {
        PointerDownEvent = new UnityEvent<Vector2>();
        PointerUpEvent = new UnityEvent();
        DragEvent = new UnityEvent<Vector2>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        PointerDownEvent.Invoke(eventData.position);
    }

    public void OnDrag(PointerEventData eventData)
    {
        DragEvent.Invoke(eventData.position);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        PointerUpEvent.Invoke();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        
    }
}