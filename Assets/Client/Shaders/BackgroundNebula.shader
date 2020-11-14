Shader "Game/Background/Nebula"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SecondTex ("Texture", 2D) = "white" {}
        _Scroll ("Scroll Speed X1,Y1,X2,Y2", Vector) = (0.1, 0.1, 0.1, 0.1)
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

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _SecondTex;
            float4 _MainTex_ST;
            float4 _SecondTex_ST;
            float4 _Scroll;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 c1 = tex2D(_MainTex, i.uv + float2(_Scroll.x, _Scroll.y) * _Time.x);
                fixed4 c2 = tex2D(_SecondTex, i.uv + float2(_Scroll.z, _Scroll.w) * _Time.x);
                fixed4 c = (c1 + c2 + c1 * c2) * 0.33;
                return c * c * 2.0;
            }
            ENDCG
        }
    }
}
