using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WindowManager : Jam.Singleton<WindowManager>
{
    public List<WindowUI> WindowList = new List<WindowUI>();
    public Transform Canvas => mCanvas;

    [Header("Settings")]
    [SerializeField] private Sprite mSpritePanel;
    [SerializeField] private int mAmountToPool;

    private List<GameObject> mPooledObjects;
    private Transform mCanvas;
    private RectTransform mRectCanvas;

    private void Awake()
    {
        base.Awake();

        mCanvas = FindObjectOfType<Canvas>().transform;
        mRectCanvas = mCanvas.GetComponent<RectTransform>();
    }

    private void Start()
    {
        mPooledObjects = new List<GameObject>();
        GameObject tmp;
        for (int i = 0; i < mAmountToPool; i++)
        {
            tmp = CreateWindow(Vector3.zero, Vector2.zero, false, false, false);
            tmp.SetActive(false);
            mPooledObjects.Add(tmp);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            PopUpWindow(ScreenPositionToCanvas(Input.mousePosition), new Vector2(400, 400));
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            PushBackWindow(WindowList.Count - 1);
        }
    }

    public void PopUpWindow(Vector3 position, Vector2 size, bool isFixed = false, bool isScreenLimited = true, bool isSnap = true)
    {
        GameObject winGO = GetPooledObject();
        if (winGO != null)
        {
            WindowUI winGOUI = winGO.GetComponent<WindowUI>();
            winGOUI.SetWindowPositionAndSize(position, size);
            winGOUI.SetSettings(isFixed, isScreenLimited, isSnap);
            winGO.SetActive(true);

            WindowList.Add(winGOUI);
        }
    }

    private GameObject GetPooledObject()
    {
        for (int i = 0; i < mAmountToPool; i++)
        {
            if (!mPooledObjects[i].gameObject.activeInHierarchy)
            {
                return mPooledObjects[i].gameObject;
            }
        }
        return null;
    }

    private GameObject CreateWindow(Vector3 position, Vector2 size, bool isFixed, bool isScreenLimited, bool isSnap)
    {
        //Create Game Object
        GameObject winGO = new GameObject();
        winGO.transform.SetParent(mCanvas, false);

        //Add Image => RectTransform, Canvas Renderer, Default material
        Image winImage = winGO.AddComponent<Image>();
        winImage.type = Image.Type.Sliced;
        winImage.sprite = mSpritePanel;

        RectTransform winRect = winGO.GetComponent<RectTransform>();
        winRect.anchorMin = new Vector2(0, 0);
        winRect.anchorMax = new Vector2(1, 1);
        winRect.anchoredPosition = new Vector2(0.5f, 0.5f);

        //Add Windows UI and configuration
        WindowUI winUI = winGO.AddComponent<WindowUI>();
        winUI.SetWindowPositionAndSize(position, size);
        winUI.SetSettings(isFixed, isScreenLimited, isSnap);

        return winGO;
    }

    public void PushBackWindow(int index)
    {
        if (WindowList.Contains(WindowList[index]))
        {
            WindowList[index].gameObject.SetActive(false);
            WindowList.RemoveAt(index);
        }
    }

    public Vector3 ScreenPositionToCanvas(Vector3 mousePos)
    {
        Vector2 worldPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
                mRectCanvas.GetComponent<RectTransform>(), Input.mousePosition, 
                mRectCanvas.GetComponent<Canvas>().renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main, out worldPos);
        return worldPos;
    }
}
