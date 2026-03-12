
Shader "Custom/UVAxeShine"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)   // SpriteRenderer Color
        _Speed ("Shine Speed", Float) = 1.0
        _Width ("Shine Width", Float) = 0.2
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _Color;   // SpriteRenderer Color
            float _Speed;
            float _Width;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv);

                // SpriteRenderer Color 곱하기
                col *= _Color;

                float shinePos = frac(_Time.y * _Speed);
                float dist = abs(i.uv.x - shinePos);      // 좌 -> 우 방향
                // float dist = abs(i.uv.x - shinePos);   // 상 -> 하 방향
                float shine = smoothstep(_Width, 0.0, dist);

                // 원래 색 유지 + 흰빛 섞기
                // col.rgb = lerp(col.rgb, float3(1.0, 1.0, 1.0), shine);

                //원래 색을 유지하면서 밝기만 살짝 올리기
                col.rgb += shine * 0.5;



                return col;
            }
            ENDCG
        }
    }
}