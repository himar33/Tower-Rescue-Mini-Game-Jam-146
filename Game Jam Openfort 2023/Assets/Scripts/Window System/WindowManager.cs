using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class WindowManager : Jam.Singleton<WindowManager>
{
    [SerializeField]
    ObjectPooler mWindowObjectPooler;
    
    public List<WindowUI> WindowList = new List<WindowUI>();
    public Transform Canvas => mCanvas;

    [Header("Settings")]
    
    [SerializeField] 
    Sprite mSpritePanel;

    [Header("Test")]
    
    [SerializeField]
    CSpellScriptableObject mTestSpell;

    [SerializeField]
    Font mFont;

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
        mWindowObjectPooler.AddConstructor(CreateWindow);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ConfigNewSpellWindow(mTestSpell);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            DisableWindow(WindowList.Count - 1);
        }
    }

    public void ConfigNewSpellWindow(CSpellScriptableObject aSpell)
    {
        Vector3 Position = ScreenPositionToCanvas();
        Vector2 Size = new Vector2(400, 400);
        bool isFixed = false;
        bool isScreenLimited = true;
        bool isSnap = true;

        GameObject winGO = mWindowObjectPooler.GetPooledObject();

        winGO.transform.localScale = Vector3.one;
        winGO.GetComponent<CWindowSpellData>().SetSpell(aSpell);
        
        WindowUI winGOUI = winGO.GetComponent<WindowUI>();
        winGOUI.SetWindowPositionAndSize(Position, Size);
        winGOUI.SetSettings(isFixed, isScreenLimited, isSnap);
        winGO.SetActive(true);

        WindowList.Add(winGOUI);
    }

    public void DisableWindow(int index)
    {
        if (WindowList.Contains(WindowList[index]))
        {
            WindowList[index].gameObject.SetActive(false);
            WindowList.RemoveAt(index);
        }
    }

    private GameObject CreateWindow()
    {
        //Create Game Object
        GameObject winGO = new GameObject("HUD Window");

        //Add Image => RectTransform, Canvas Renderer, Default material
        Image winImage = winGO.AddComponent<Image>();
        winImage.type = Image.Type.Sliced;
        winImage.sprite = mSpritePanel;

        RectTransform winRect = winGO.GetComponent<RectTransform>();
        winRect.anchorMin = new Vector2(0, 0);
        winRect.anchorMax = new Vector2(1, 1);
        winRect.anchoredPosition = new Vector2(0.5f, 0.5f);

        winGO.AddComponent<WindowUI>();

        GameObject SpellImageObject = new GameObject("Spell Info Image");
        SpellImageObject.transform.SetParent(winGO.transform);
        var InfoImageComponent = SpellImageObject.AddComponent<Image>();

        GameObject SpellTextObject = new GameObject("Spell Info Text");
        SpellTextObject.transform.SetParent(winGO.transform);
        var TextComponent = SpellTextObject.AddComponent<Text>();
        TextComponent.color = Color.black;
        TextComponent.font = mFont;

        var SpellInfoComponent = winGO.AddComponent<CWindowSpellData>();
        SpellInfoComponent.SetUIElements(InfoImageComponent, TextComponent);

        return winGO;
    }

    public static Vector3 ScreenPositionToCanvas()
    {
        Vector2 worldPos;
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
                Instance.mRectCanvas.GetComponent<RectTransform>(), 
                Input.mousePosition, 
                Instance.mRectCanvas.GetComponent<Canvas>().renderMode == RenderMode.ScreenSpaceOverlay ? null : Camera.main, out worldPos);
        
        return worldPos;
    }
}
