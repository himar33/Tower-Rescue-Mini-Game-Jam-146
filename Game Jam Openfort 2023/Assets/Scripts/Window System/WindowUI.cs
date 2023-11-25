using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WindowUI : MonoBehaviour, IDragHandler, IPointerDownHandler
{
    [Header("Settings")]
    [SerializeField] private bool mIsFixed;
    [SerializeField] private bool mIsScreenLimited;
    [SerializeField] private bool mIsSnap;

    private const float BORDER_SNAP_SIZE = 20.0f;

    private Vector3 mMouseDragStartPos;
    private PointerEventData.InputButton mMouseButton;
    private RectTransform mRectTransform;

    public void OnDrag(PointerEventData eventData)
    {
        if (eventData.button == mMouseButton)
        {
            mRectTransform.localPosition = Input.mousePosition - mMouseDragStartPos;
            if (mIsScreenLimited) CheckScreenBorder();
            if (mIsSnap) SnapEachOther();
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == mMouseButton)
        {
            mMouseDragStartPos = Input.mousePosition - mRectTransform.localPosition;
        }
        transform.SetAsLastSibling();
    }

    public void SetWindowPositionAndSize(Vector3 position, Vector2 size)
    {
        mRectTransform.localPosition = position;

        mRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
        mRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
    }

    public void SetSettings(bool isFixed, bool isScreenLimited, bool isSnap)
    {
        mIsFixed = isFixed;
        mIsScreenLimited = isScreenLimited;
        mIsSnap = isSnap;
    }

    private void Awake()
    {
        mRectTransform = GetComponent<RectTransform>();
    }

    private void CheckScreenBorder()
    {
        Vector3 diffMin = mRectTransform.position + (Vector3)mRectTransform.rect.position;
        Vector3 diffMax = (Vector3)Camera.main.pixelRect.size
            - mRectTransform.position
            + (Vector3)mRectTransform.rect.position;

        if (diffMin.x < BORDER_SNAP_SIZE)
            mRectTransform.position -= new Vector3(diffMin.x, 0f, 0f);
        if (diffMin.y < BORDER_SNAP_SIZE)
            mRectTransform.position -= new Vector3(0f, diffMin.y, 0f);
        if (diffMax.x < BORDER_SNAP_SIZE)
            mRectTransform.position += new Vector3(diffMax.x, 0f, 0f);
        if (diffMax.y < BORDER_SNAP_SIZE)
            mRectTransform.position += new Vector3(0f, diffMax.y, 0f);
    }

    private void SnapEachOther()
    {
        foreach (WindowUI window in WindowManager.Instance.WindowList)
        {
            if (window == this) continue;
            if (window.gameObject.activeInHierarchy)
            {
                Vector3 DiffMin = window.mRectTransform.position
                    - (Vector3)(window.mRectTransform.rect.position + mRectTransform.rect.position)
                    - mRectTransform.position;

                Vector3 DiffMax = window.mRectTransform.position
                    + (Vector3)(window.mRectTransform.rect.position + mRectTransform.rect.position)
                    - mRectTransform.position;

                if (Mathf.Abs(DiffMin.x) < BORDER_SNAP_SIZE)
                    mRectTransform.position += new Vector3(DiffMin.x, 0.0f, 0.0f);
                if (Mathf.Abs(DiffMin.y) < BORDER_SNAP_SIZE)
                    mRectTransform.position += new Vector3(0.0f, DiffMin.y, 0.0f);
                if (Mathf.Abs(DiffMax.x) < BORDER_SNAP_SIZE)
                    mRectTransform.position += new Vector3(DiffMax.x, 0.0f, 0.0f);
                if (Mathf.Abs(DiffMax.y) < BORDER_SNAP_SIZE)
                    mRectTransform.position += new Vector3(0.0f, DiffMax.y, 0.0f);
            }
        }
    }
}
