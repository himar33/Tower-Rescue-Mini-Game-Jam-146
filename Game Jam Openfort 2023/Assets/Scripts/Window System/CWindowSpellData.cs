using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


public class CWindowSpellData : MonoBehaviour
{
    CSpellScriptableObject mSpellData;

    Image mHUDSpellInfoObject;

    public void SetSpell(CSpellScriptableObject aSpellData)
    {
        mSpellData = aSpellData;

        mHUDSpellInfoObject.sprite = aSpellData.mHUDSprite;
    }
    
    public CSpellScriptableObject GetSpell()
    {
        return mSpellData;
    }

    public void SetUIElement(Image aHUDSpellInfoObject)
    {
        mHUDSpellInfoObject = aHUDSpellInfoObject;
    }
}
