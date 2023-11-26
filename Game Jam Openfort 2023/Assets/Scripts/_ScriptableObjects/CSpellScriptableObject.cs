using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CSpellScriptableObject", menuName = "GameJam/Spell", order = 0)]
public class CSpellScriptableObject : ScriptableObject 
{
    public string mDescription;
    
    public Texture mStencilTexture;
    
    public Sprite mHUDSprite;
    
    public float mActiveTimeSeconds;

    public float mFadeSeconds = 0.5f;

    [Header("Lights")]
    public Vector2 mLightPosition01;

    public float mLightIntensity;

    public Color mLightColor;
}
