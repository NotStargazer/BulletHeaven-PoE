Shader "VFX/FireExplosion"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _ExTime ("Time", Range(0, 1)) = 1
        _Radius ("Radius", Range(0, 1)) = 1
        _Offset ("Outer Offset", Range(0, 1)) = 0.2
        _FireMoveParams ("Fire Params", Vector) = (0,0.2,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
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

            float4 _Color;
            float4 _FireMoveParams;
            float _ExTime;
            float _Radius;
            float _Offset;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float rand(float2 co)
            {
                return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
            }

            float2 hash(float2 p)
            {
                p = float2(dot(p, float2(127.1, 311.7)),
                           dot(p, float2(269.5, 183.3)));

                return -1.0 + 2.0 * frac(sin(p) * 43758.5453123);
            }

            float noise(in float2 p)
            {
                const float k1 = 0.366025404; // (sqrt(3)-1)/2;
                const float k2 = 0.211324865; // (3-sqrt(3))/6;

                float2 i = floor(p + (p.x + p.y) * k1);

                float2 a = p - i + (i.x + i.y) * k2;
                float2 o = step(a.yx, a.xy);
                float2 b = a - o + k2;
                float2 c = a - 1.0 + 2.0 * k2;

                float3 h = max(0.5 - float3(dot(a, a), dot(b, b), dot(c, c)), 0.0);

                float3 n = h * h * h * h * float3(dot(a, hash(i + 0.0)), dot(b, hash(i + o)), dot(c, hash(i + 1.0)));

                return dot(n, (70.0).xxx);
            }

            float fbm(float2 p)
            {
                float f = 0.0;
                matrix<float, 2, 2> m = matrix<float, 2, 2>(1.6, 1.2, -1.2, 1.6);

                f = 0.5000 * noise(p);
                p = mul(m, p);
                f += 0.2500 * noise(p);
                p = mul(m, p);
                f += 0.1250 * noise(p);
                p = mul(m, p);
                f += 0.0625 * noise(p);
                p = mul(m, p);
                f = 0.5 + 0.5 * f;
                return f;
            }

            fixed4 frag (v2f i) : SV_Target
            {    
                float2 cuv = abs(i.uv * 2 - 1);
                float uvT = i.uv * float2(0.75, 0.2) + _Time.y * _FireMoveParams.xy;
                float n = pow(fbm(2.0 * uvT), 1.0);
                
                float d = length(cuv);
                float r1 = lerp(-_Radius - _Offset, 1, _ExTime);
                float r2 = lerp(-_Offset, 1 + _Radius, _ExTime);
                float v = smoothstep(r1, r2, d);
                v -= smoothstep(r2 - 0.001, r2 + _Offset, d);
                //v = frac(v);
                
                float gradient = pow(1.0 - cuv.y, 1.5) * 5.;
                float finalNoise = n * gradient;
                float3 color = finalNoise * float3(2.*n, 2.*n*n*n, n*n*n*n);
                
                
                return float4(color, gradient);
            }
            ENDCG
        }
    }
}
