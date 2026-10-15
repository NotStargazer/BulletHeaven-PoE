Shader "UI/HSV grab pass"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent" }
        LOD 100

        GrabPass
        {
            "_Grab"
        }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 color : COLOR;
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _Grab;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            float4x4 brightnessMatrix(const float brightness)
            {
                return float4x4(1, 0, 0, 0,
                                0, 1, 0, 0,
                                0, 0, 1, 0,
                                brightness, brightness, brightness, 1);
            }

            float4x4 contrastMatrix(const float contrast)
            {
                const float t = (1.0 - contrast) / 2.0;
                return float4x4(contrast, 0, 0, 0,
                                0, contrast, 0, 0,
                                0, 0, contrast, 0,
                                t, t, t, 1);
            }

            float4x4 saturationMatrix(const float saturation)
            {
                const float3 luminance = float3(0.3086, 0.6094, 0.0820);
                const float oneMinusSat = 1.0 - saturation;
                const float3 red = float3((luminance.x * oneMinusSat) .xxx) + float3(saturation, 0, 0);
                const float3 green = float3((luminance.y * oneMinusSat) .xxx) + float3(0, saturation, 0);
                const float3 blue = float3((luminance.z * oneMinusSat) .xxx) + float3(0, 0, saturation);

                return float4x4(red, 0,
                                green, 0,
                                blue, 0,
                                0, 0, 0, 1);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 c = LinearToGammaSpace(i.color);
                float4 col = tex2D(_Grab, float2(i.uv.x, 1 - i.uv.y));
                col = mul(col, brightnessMatrix(c.x * 2 - 1));
                col = mul(col, contrastMatrix(c.y * 2 - 1));
                col = mul(col, saturationMatrix(c.z));
                return col;
            }
            ENDCG
        }
    }
}
