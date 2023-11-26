Shader "Unlit/MaskPreview"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Noise ("Noise", 2D) = "white" {}
        _HighlightColor ("Highlight Color", Color) = (1,1,1,1)
        _EdgeThickness ("Edge Thickness", Range(0,1)) = 0.1
        _MinAlpha ("Minimum Alpha", Range(0,1)) = 0.3
        _BlinkSpeed ("BlinkSpeed", float) = 1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" }

        ZWrite False
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Interpolators
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _Noise;
            float4 _Noise_ST;
            float4 _HighlightColor;
            float _EdgeThickness;
            float _BlinkSpeed;
            float _MinAlpha;

            float _MaskWidth;
            float _MaskHeight;

            static const float _RefScreenWidth = 1920.0f;
            static const float _RefMaskSize = 1.0f;  

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                float Grayscale = 1 - tex2D(_MainTex, i.uv).r;

                float2 NoiseUVs = i.uv * _Noise_ST.xy + _Noise_ST.zw + float2(1,1) * _Time.y * _BlinkSpeed;
                float NoiseSample = tex2D(_Noise, NoiseUVs).r;

                float EdgeScreenSizeMultiplier = _RefScreenWidth / _ScreenParams.y;
                float EdgeMaskWidthMultiplier = _RefMaskSize / _MaskWidth;                
                float EdgeMaskHeightMultiplier = _RefMaskSize / _MaskHeight;

                float EdgeCompoMultiplier = EdgeScreenSizeMultiplier * min(EdgeMaskWidthMultiplier, EdgeMaskHeightMultiplier);

                float Thickness = _EdgeThickness / 2.0f * saturate(EdgeCompoMultiplier);
                // Thickness = 0.1;
                float AlphaEdge = 0.5f;

                float SDFUnclamped = abs(Grayscale - AlphaEdge);
                float SDF = 1 - (SDFUnclamped) / (Thickness);
                float Edge = saturate(SDF) * saturate(NoiseSample + _MinAlpha);

                float Blink = abs(sin(_Time.y * _BlinkSpeed));

                float4 Highlight = lerp(_HighlightColor, float4(_HighlightColor.rgb, _MinAlpha), Blink);
                
                float4 Composition = Edge * Highlight;

                return Composition;
            }
            ENDCG
        }
    }
}
