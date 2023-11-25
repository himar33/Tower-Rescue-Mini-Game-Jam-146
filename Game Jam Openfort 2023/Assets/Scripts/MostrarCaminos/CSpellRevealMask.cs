using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CSpellRevealMask : MonoBehaviour
{
    CSpellScriptableObject mSpellInfo;
    float mCurrentTime;

    MaterialPropertyBlock mpb;

    MaterialPropertyBlock MPB
    {
        get
        {
            if(mpb == null)
                mpb = new MaterialPropertyBlock();
            
            return mpb;
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

        if(mSpellInfo != null && mCurrentTime >= mSpellInfo.mActiveTimeSeconds)
        {
            gameObject.SetActive(false);
            mCurrentTime = 0;
        }
    }
}
