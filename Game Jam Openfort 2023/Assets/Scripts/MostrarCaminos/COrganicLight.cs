using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class COrganicLight : MonoBehaviour
{
    static readonly float mHueMaxDiff = 0.01f;
    static readonly float mValueMaxDiff = 0.5f;
    static readonly float mHueSpeed = 5.0f;
    static readonly float mValueSpeed = 2.0f;
    
    Light mLightComponent;
    Color mInitialColor;

    public void Start() 
    {
        mLightComponent = GetComponent<Light>();
    }

    public void SetColor(Color aNewColor)
    {
        mInitialColor = aNewColor;
    }

    public void Update() 
    {
        float NoiseX = (Mathf.PerlinNoise(Time.realtimeSinceStartup * mHueSpeed, 0) - 0.5f) * 2.0f;
        float NoiseY = (Mathf.PerlinNoise(0, Time.realtimeSinceStartup * mValueSpeed) - 0.5f) * 2.0f;
 
        Color.RGBToHSV(mInitialColor, out float Hue, out float Saturation, out float Value);

        Hue += NoiseX * mHueMaxDiff;
        Value += NoiseY * mValueMaxDiff;

        mLightComponent.color = Color.HSVToRGB(Hue, Saturation, Value);
        
    }
}
