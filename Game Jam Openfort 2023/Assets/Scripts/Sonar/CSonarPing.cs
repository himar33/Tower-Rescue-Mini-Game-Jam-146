using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSonarPing : MonoBehaviour
{
    const float mAppearTime = 0.15f;
    float mCurrentTime;
    float mDuration;
    float mMaxScale;

    public void Start() 
    {
        mMaxScale = transform.localScale.x;
        transform.localScale = Vector3.zero;
    }
    
    public void InitShow(float aStartDelay, float aDuration)
    {
        mCurrentTime = -aStartDelay;
        mDuration = aDuration - aStartDelay;
    }

    public void Update()
    {
        mCurrentTime += Time.deltaTime;
        
        if(mCurrentTime <= mAppearTime)
        {
            float Scale01 = Mathf.Clamp01(mCurrentTime / mAppearTime);
            transform.localScale = Vector3.one * Scale01 * mMaxScale;;
        }
        else if (mCurrentTime > mDuration - mAppearTime)
        {
            float Scale01 = 1 - Mathf.Clamp01( (mCurrentTime - (mDuration - mAppearTime))/(mDuration - (mDuration - mAppearTime)));
            transform.localScale = Vector3.one * Scale01 * mMaxScale;
        }

        if(mCurrentTime > mDuration)
        {
            this.gameObject.SetActive(false);
        }

    }
}
