using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class CWindowSpellData : MonoBehaviour
{
    CSpellScriptableObject mSpellData;

    Image mHUDSpellInfoObject;
    Text mTextObject;

    public void SetSpell(CSpellScriptableObject aSpellData)
    {
        mSpellData = aSpellData;

        mTextObject.text = aSpellData.mDescription;
        // mHUDSpellInfoObject.sprite = aSpellData.mStencilTexture;
    }
    
    public CSpellScriptableObject GetSpell()
    {
        return mSpellData;
    }

    public void SetUIElements(Image aHUDSpellInfoObject, Text aTextObject)
    {
        mHUDSpellInfoObject = aHUDSpellInfoObject;
        mTextObject = aTextObject;
    }
}
