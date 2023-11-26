using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class CCastSpellManager : MonoBehaviour
{
    public static CCastSpellManager instance;

    [SerializeField]
    Transform mFocusSpellTransform;

    [SerializeField]
    float mDetectCastSqrDistance;

    [SerializeField]
    float mOnFocusWindowScale;
    
    [SerializeField]
    float mSnapToFocusTime;
    
    [SerializeField]
    CRevealPath mRevealPathComponent;

    [Header("Images")]
    
    [SerializeField] 
    Sprite mSpriteStandby;

    [SerializeField] 
    Sprite mSpriteLit;


    bool mIsSnapped;
    GameObject mDragWindow;
    GameObject mSpellOnFocus;
    float mCurrentSnapTime;


    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void Start() 
    {
        mRevealPathComponent.OnMaskCreated += ConsumeSpell;
        mRevealPathComponent.enabled = false;
    }
    
    public void NotifyDrag(GameObject aWindow)
    {
        mDragWindow = aWindow;
        mRevealPathComponent.enabled = false;
    }

    public void NotifyUnclick(GameObject aWindow)
    {
        float SqrMagnitude = (mDragWindow.transform.position - mFocusSpellTransform.position).sqrMagnitude;
        
        if(mDragWindow == aWindow && SqrMagnitude < mDetectCastSqrDistance)
        {
            if(mSpellOnFocus && mSpellOnFocus != aWindow && !mIsSnapped)
            {
                mSpellOnFocus.transform.localScale = Vector3.one;
            }

            if(mSpellOnFocus == null && !mIsSnapped)
            {
                mSpellOnFocus = mDragWindow;
                mDragWindow = null;                
                mIsSnapped = true;
            }
        }
        else if(mSpellOnFocus && mSpellOnFocus == mDragWindow)
        {
            mIsSnapped = false;
            mRevealPathComponent.enabled = false;
            Debug.Log("unfocus");
            mSpellOnFocus.GetComponent<Image>().sprite = mSpriteStandby;
            mCurrentSnapTime = 0;
        }

        if(mSpellOnFocus && mIsSnapped)
        {
            CastSpell();
        }

        mDragWindow = null;
    }

    private void Update() 
    {
        if(mSpellOnFocus && mCurrentSnapTime < mSnapToFocusTime)
        {
            mCurrentSnapTime += Time.deltaTime;

            float Snap01 = Mathf.Clamp01(mCurrentSnapTime / mSnapToFocusTime);

            if(mIsSnapped)
                mSpellOnFocus.transform.localScale = Vector3.one * Mathf.Lerp(1, mOnFocusWindowScale, Snap01);
            else
                mSpellOnFocus.transform.localScale = Vector3.one * Mathf.Lerp(mOnFocusWindowScale, 1, Snap01);

            if(!mIsSnapped && Snap01 == 1)
            {
                mSpellOnFocus = null;
                mCurrentSnapTime = 0;
            }
        }
    }

    private void CastSpell()
    {
        mRevealPathComponent.enabled = true;
        mRevealPathComponent.SetActiveSpell(mSpellOnFocus.GetComponent<CWindowSpellData>().GetSpell());
        mSpellOnFocus.GetComponent<Image>().sprite = mSpriteLit;
    }

    public void ConsumeSpell()
    {
        mRevealPathComponent.enabled = false;
        mSpellOnFocus.GetComponent<Image>().sprite = mSpriteStandby;
        mSpellOnFocus.SetActive(false);
        mSpellOnFocus = null;
        mIsSnapped = false;
        mCurrentSnapTime = 0;
    }
}
