using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowUI : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    private Vector3 mMouseDragStartPos;
    public void OnDrag(PointerEventData eventData)
    {
        transform.localPosition = Input.mousePosition - mMouseDragStartPos;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        mMouseDragStartPos = Input.mousePosition - transform.localPosition;
    }

}
