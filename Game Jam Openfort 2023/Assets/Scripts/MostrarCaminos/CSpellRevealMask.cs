using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CSpellRevealMask : MonoBehaviour
{
    
    CSpellScriptableObject mSpellInfo;
    float mCurrentTime;

    MaterialPropertyBlock mPropertyBlock;

    MaterialPropertyBlock MPB
    {
        get
        {
            if(mPropertyBlock == null)
                mPropertyBlock = new MaterialPropertyBlock();
            
            return mPropertyBlock;
        }
    }
    
    public void Config(CSpellScriptableObject aSpellInfo) 
    {
        mSpellInfo = aSpellInfo;

        MPB.SetTexture("_MainTex", aSpellInfo.mStencilTexture);
        GetComponent<MeshRenderer>().SetPropertyBlock(MPB);

        mCurrentTime = 0;
    }

    public void Update()
    {
        mCurrentTime += Time.deltaTime;

        if(mCurrentTime < mSpellInfo.mFadeSeconds)
        {
            float Alpha01 = (mCurrentTime / mSpellInfo.mFadeSeconds);
            MPB.SetFloat("_Alpha", Alpha01);
            GetComponent<MeshRenderer>().SetPropertyBlock(MPB);
        }

        if(mCurrentTime > mSpellInfo.mActiveTimeSeconds - mSpellInfo.mFadeSeconds)
        {
            float minValue = mSpellInfo.mActiveTimeSeconds - mSpellInfo.mFadeSeconds;
            float Alpha01 = 1 - (mCurrentTime - minValue) / (mSpellInfo.mActiveTimeSeconds - minValue);
            
            MPB.SetFloat("_Alpha", Alpha01);
            
            GetComponent<MeshRenderer>().SetPropertyBlock(MPB);
        }

        if(mSpellInfo != null && mCurrentTime >= mSpellInfo.mActiveTimeSeconds)
        {
            gameObject.SetActive(false);
            mCurrentTime = 0;
        }
    }
}
