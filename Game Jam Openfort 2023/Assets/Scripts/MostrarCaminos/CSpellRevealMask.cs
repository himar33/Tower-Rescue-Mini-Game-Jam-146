using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class CSpellRevealMask : MonoBehaviour
{
    CSpellScriptableObject mSpellInfo;
    
    float mCurrentTime;

    Light mPointLight;

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
    
    public void ConfigSpell(CSpellScriptableObject aSpellInfo, Vector2 aDimensions) 
    {
        mSpellInfo = aSpellInfo;

        MPB.SetTexture("_MainTex", aSpellInfo.mStencilTexture);
        GetComponent<MeshRenderer>().SetPropertyBlock(MPB);

        CreateLight();

        mPointLight.intensity = aSpellInfo.mLightIntensity;

        mPointLight.GetComponent<COrganicLight>().SetColor(aSpellInfo.mLightColor);

        mCurrentTime = 0;
    }

    private void CreateLight()
    {
        if(mPointLight == null)
        {
            GameObject mPointLightObject = new GameObject("Point Light"); 
            mPointLightObject.transform.SetParent(this.transform);
            
            mPointLight = mPointLightObject.AddComponent<Light>();
            mPointLight.shadows = LightShadows.Hard;
            mPointLightObject.AddComponent<COrganicLight>();
            
            mPointLight.type = LightType.Point;
        }
    }

    public void ConfigLight(Vector3 aLightPosition, float aSize)
    {
        mPointLight.transform.position = aLightPosition;
        mPointLight.range = aSize;
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
