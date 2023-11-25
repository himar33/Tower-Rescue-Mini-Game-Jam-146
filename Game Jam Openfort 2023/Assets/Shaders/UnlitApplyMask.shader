Shader "Unlit/UnlitApplyMask"
{
    Properties
    {
        _MaskBuffer("Mask Buffer", 2D) = "black" {}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }

        Pass
        {
            Stencil
            {
                Ref 1
                Comp notequal
                Pass Zero
            }

            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

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
                float4 vertex : SV_POSITION;
                float4 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            sampler2D _MaskBuffer;
            float4 _MaskBuffer_ST;

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.screenPos = ComputeScreenPos(o.vertex);
                return o;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                float2 UVs = i.screenPos.xy / i.screenPos.w;
                float Alpha = 1 - tex2D(_MaskBuffer, UVs);
                return float4(0,0,0, Alpha);
            }
            ENDCG
        }
    }
}
