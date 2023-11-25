using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CSpellScriptableObject", menuName = "GameJam/Spell", order = 0)]
public class CSpellScriptableObject : ScriptableObject 
{
    public string mDescription;
    
    public Texture mStencilTexture;
    
    public Vector2 mLightPosition01;

    public float mActiveTimeSeconds;
}
