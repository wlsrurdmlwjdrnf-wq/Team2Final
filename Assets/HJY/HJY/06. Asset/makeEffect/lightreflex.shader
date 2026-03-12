

Shader "Custom/UVGradientShine"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
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

                // 빛의 위치를 시간에 따라 이동
                float shinePos = frac(_Time.y * _Speed);

                // UV.x 기준으로 빛의 라인 생성
                float dist = abs(i.uv.x - shinePos);

                // Gradient: 중심은 흰색, 주변은 투명
                float shine = smoothstep(_Width, 0.0, dist);

                // Additive로 빛을 더해줌
                col.rgb += shine;

                return col;
            }
            ENDCG
        }
    }
}