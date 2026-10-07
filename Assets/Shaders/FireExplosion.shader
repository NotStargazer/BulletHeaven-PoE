Shader "VFX/FireExplosion"
{
    Properties
    {
        [HDR] _Color ("Color", Color) = (1,1,1,1)
        _ExTime ("Time", Range(0, 1)) = 1
        _Radius ("Radius", Range(0, 1)) = 1
        _Offset ("Outer Offset", Range(0, 1)) = 0.2
        _FireMoveParams ("Fire Params", Vector) = (0,0.2,1,1)
    }
    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha
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
            float _Seed;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float snoise(float3 uv, float res)
            {
	            const float3 s = float3(1e0, 1e2, 1e3);

	            uv *= res;

                float3 uv0 = floor(fmod(uv, res)) * s;
	            float3 uv1 = floor(fmod(uv + 1 .xxx, res)) * s;

	            float3 f = frac(uv);
	            f = f * f * (3.0 - 2.0 * f);

	            float4 v = float4(uv0.x + uv0.y + uv0.z, uv1.x + uv0.y + uv0.z,
	                              uv0.x + uv1.y + uv0.z, uv1.x + uv1.y + uv0.z);

	            float4 r = frac(sin(v * 1e-1) * 1e3);
	            float r0 = lerp(lerp(r.x, r.y, f.x), lerp(r.z, r.w, f.x), f.y);

	            r = frac(sin((v + uv1.z - uv0.z) * 1e-1) * 1e3);
	            float r1 = lerp(lerp(r.x, r.y, f.x), lerp(r.z, r.w, f.x), f.y);

	            return lerp(r0, r1, f.z) * 2. - 1.;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {    
                float seedTime = _Time.y + _Seed * 126;
                float2 cuv = i.uv * 2 - 1;
                float d = length(cuv);
                float d2 = length(cuv * 2);
                float a = atan2(cuv.x, cuv.y) / UNITY_TWO_PI;
                
                float r = lerp(-_Offset, 1 + _Radius, _ExTime);
                float v = smoothstep(r - _Radius, r, d);
                v -= smoothstep(r - 0.001, r + _Offset, d);

                float color = _FireMoveParams.x - _FireMoveParams.y * d2;

                float3 coord = float3(a + .5, d * .4, .5);

                [unroll]
                for (int index = 1; index < 8; index++)
                {
	                float power = pow(2, float(index));
	                color += 1.5 / power * snoise(coord + float3(0, - seedTime * _FireMoveParams.z, seedTime * _FireMoveParams.w), power * 12);
                }

                color *= v;
                return float4(color, pow(max(color, 0), 2) * _Color.g, pow(max(color, 0), 3) * _Color.b, _Color.a * color);
            }
            ENDCG
        }
    }
}
