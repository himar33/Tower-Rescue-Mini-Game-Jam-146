using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSonarPing : MonoBehaviour
{
    const float mAppearTime = 0.15f;
    float mCurrentTime;

    public void Start() 
    {
        transform.localScale = Vector3.zero;
    }
    
    public void InitShow(float mStartDelay)
    {
        mCurrentTime = -mStartDelay;
    }

    public void Update()
    {
        if(mCurrentTime <= mAppearTime)
        {
            mCurrentTime += Time.deltaTime;

            float Scale01 = Mathf.Clamp01(mCurrentTime / mAppearTime);
            transform.localScale = Vector3.one * Scale01;
        }
    }
}
