Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Power ("Power", Range(0.0, 1.0)) = 0.5
        _TopColor ("TopColor", Color) = (0, 0, 0, 0)
        _BottomColor ("BottomColor", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _Power;
            float4 _TopColor, _BottomColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 output;

                float4 color = lerp(_BottomColor, _TopColor, i.uv.y);

                // return i.uv.y < _Power;

                output = color * (i.uv.y < _Power);
                return output;
            }
            ENDCG
        }
    }
}
