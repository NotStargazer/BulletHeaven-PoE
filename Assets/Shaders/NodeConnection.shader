Shader "UI/NodeConnection"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off ZWrite Off

        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "PreviewType"="Plane"
        }

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            sampler2D _MainTex;
            float _WidthRatio;
            float _Radius;
            float _Range;

            float easeInCirc(float t)
            {
                return sqrt(1 - pow(t - 1, 2));
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 cx = float2((i.uv.x - 0.5f) * 2, lerp(1 - i.uv.y, i.uv.y, (sign(_Radius) + 1) / 2));
                cx.x /= 1 / _Radius;
                float f = easeInCirc(1 - _Radius);
                cx.y += f - _WidthRatio * f;
                float l = length(cx);
                float a = step(1 - _WidthRatio, l) - step(1, l);

                float x = smoothstep(1 - _WidthRatio, 1, l);
                float y = y = i.uv.x;

                if (_Radius != 0)
                {
                    y = atan2(cx.x, lerp(cx.y, 1, f)) / UNITY_PI;
                    y /= _Radius;
                    y += 0.5f;
                    y = lerp(1 - y, y, (sign(_Radius) + 1) / 2);
                }

                float2 uv = float2(x, y);
                //return float4(uv, 0, 1);
                fixed4 col = tex2D(_MainTex, uv);
                col.a *= a;
                return col;
            }
            ENDCG
        }
    }
}
