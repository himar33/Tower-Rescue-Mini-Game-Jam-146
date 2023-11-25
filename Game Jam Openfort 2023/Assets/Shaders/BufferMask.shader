Shader "Unlit/BufferMask"
{
    Properties
    {
        _MainTex("Stencil", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderQueue"="Opaque" }

        Cull Off
        Blend One One

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

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (Interpolators i) : SV_Target
            {
                float4 MaskColor = tex2D(_MainTex, i.uv);
                return MaskColor;
            }
            ENDCG
        }
    }
}
