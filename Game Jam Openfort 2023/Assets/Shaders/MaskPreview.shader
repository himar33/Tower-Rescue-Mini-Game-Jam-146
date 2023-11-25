Shader "Unlit/MaskPreview"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
            float4 _HighlightColor;
            float _EdgeThickness;
            float _BlinkSpeed;
            float _MinAlpha;

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

                float RefWidth = 1920;
                float ThicknessMultiplier = _ScreenParams.y / RefWidth;

                float Thickness = _EdgeThickness / 2.0f * ThicknessMultiplier;

                float AlphaEdge = 0.5f;

                float SDFUnclamped = abs(Grayscale - AlphaEdge);
                float SDF = 1 - (SDFUnclamped) / (Thickness);
                float Edge = saturate(SDF);

                float4 Highlight = lerp(_HighlightColor, float4(_HighlightColor.rgb, _MinAlpha), abs(sin(_Time.y * _BlinkSpeed)));
                
                float4 Composition = Edge * Highlight;

                return Composition;
            }
            ENDCG
        }
    }
}
