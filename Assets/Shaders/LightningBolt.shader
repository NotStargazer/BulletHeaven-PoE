Shader "VFX/LightningBolt"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Width ("Width", Float) = 1
        _Length ("Length", Float) = 1
        _Frequency ("Frequency", Float) = 1
        _Seed ("Seed", Integer) = 0
        _Bolts ("Bolts", Integer) = 1
        _BoltDecay ("Decay Per Bolt", Range(0, 1)) = 0.5
        _StrikeWidth ("Strike Width", Range(0, 1)) = 0.1
        _GlowWidth ("Glow Width", Range(0, 1)) = 0.01
        _WidthVariance ("Variance", Vector) = (0, 0, 0, 0)
        _Formation ("Formation", Vector) = (1, 1, 1, 0)
        _Fade ("Cutoff", Vector) = (1, 1, 1, 0)
        [HDR] _Color ("Color", Color) = (1,1,1,1)
    }
    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha
        ZTest Off Cull Off Lighting Off ZWrite Off
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "PreviewType"="Plane"
        }
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

            float rand(float x)
            {
                return frac(sin(x) * 75154.32912);
            }

            float noise(float x)
            {
                float i = floor(x);
                float a = rand(i), b = rand(i + 1.);
                float f = x - i;
                return lerp(a, b, f);
            }

            float perlin(float x)
            {
                float r = 0., s = 1., w = 1.;
                for (int i = 0; i < 6; i++)
                {
                    s *= 2.0;
                    w *= 0.5;
                    r += w * noise(s * x);
                }
                return r;
            }

            float _Width;
            float _Length;
            float _Frequency;
            float _StrikeWidth;
            float _GlowWidth;
            float _BoltDecay;
            float4 _WidthVariance;
            float4 _Formation;
            float4 _Fade;

            float f(float y, float w)
            {
                return w * (perlin(2. * y) - 0.5);
            }

            float easeInCirc(float t)
            {
                return sqrt(1 - pow(t - 1, 2));
            }

            float plot(float2 p, float l, float c, float w)
            {
                return smoothstep(0, abs(f(p.y, w) - p.x / c), l / c);
            }

            sampler2D _MainTex;
            float4 _MainTex_ST;
            int _Seed;
            int _Bolts;
            float4 _Color;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 cuv = 2 * i.uv - 1;
                float2 cuv2 = cuv;
                float d = 1 - abs(cuv.y);
                d = easeInCirc(d);
                float formation = smoothstep(0, abs((cuv.y + _Formation.z) - _Formation.x), _Formation.y * _Length);;
                cuv.y *= _Frequency;
                cuv.y += _Seed * 2;

                float4 col = 0;

                for (int bolt = 1; bolt <= _Bolts; bolt++)
                {
                    float part = bolt / (uint)_Bolts;
                    float range = lerp(0 - _WidthVariance.x - _GlowWidth, 1, _StrikeWidth);
                    float strikeWidth = range + lerp(-_WidthVariance.x, _WidthVariance.y, perlin(cuv.y * _WidthVariance.z));
                    strikeWidth *= lerp(1, 1 - _BoltDecay, part);
                    float mergeWidth = range + lerp(-_WidthVariance.x, _WidthVariance.y, perlin(cuv.y * _WidthVariance.w));
                    mergeWidth *= lerp(1, 1 - _BoltDecay, part);
                    strikeWidth *= formation;
                    _GlowWidth *= formation;

                    float strike1 = plot(cuv + float2(min(mergeWidth / 2, strikeWidth / 2), bolt - 1), strikeWidth / 2, d, _Width);
                    float strike2 = plot(cuv - float2(min(mergeWidth / 2, strikeWidth / 2), -(bolt - 1)), strikeWidth / 2, d, _Width);
                    float glow = plot(cuv + float2(0, bolt - 1), strikeWidth + _GlowWidth, d, _Width);

                    float lightning = strike1 * 0.7f + strike2 * 0.7f + glow * 0.4f;
                    lightning *= formation * lerp(1, 1 - _BoltDecay, part);
                    const float r = _Fade.y + _Fade.x;
                    lightning *= abs(cuv2.y) <= _Fade.y ? 1 : smoothstep(_Fade.y, abs(cuv2.y), r);
                    col += _Color / (1 - strikeWidth) * lightning;
                }


                return col;
            }
            ENDCG
        }
    }
}
