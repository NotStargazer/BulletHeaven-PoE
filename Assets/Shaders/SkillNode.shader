Shader "Unlit/SkillNode"
{
    Properties
    {
        _FrameTex ("Frame Texture", 2D) = "white" {}
        _FrameHighlightedTex ("Node Highlighted Texture", 2D) = "white" {}
        _MainTex ("Node Texture", 2D) = "black" {}
        _ImageSize ("Image Size", Range(0, 1)) = 1
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
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 vertex : SV_POSITION;
            };

            sampler2D _FrameTex;
            float4 _FrameTex_ST;
            sampler2D _FrameHighlightedTex;
            float4 _FrameHighlightedTex_ST;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _ImageSize;

            v2f vert(appdata v)
            {
                v2f o;
                o.color = v.color;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _FrameTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float am = 0;
                float2 cuv = i.uv;
                if (_ImageSize != 0)
                {
                    cuv = (i.uv - 0.5f) / _ImageSize;
                    float l = length(cuv);
                    am = step(0.5, l);
                    cuv += 0.5f;
                }

                fixed4 frame = tex2D(_FrameTex, i.uv);
                fixed4 frameHighlight = tex2D(_FrameHighlightedTex, i.uv);
                fixed4 node = tex2D(_MainTex, cuv);
                
                frame = lerp(frame, frameHighlight, i.color.g * (1 - clamp((sin(_Time.a * 1.25) + 1) / 2, 0, 1) * i.color.b));
                frame.rgb *= i.color.r;
                node.rgb *= i.color.r;
                node.a *= am;
                
                float3 col = lerp(node, frame, clamp(node.a + frame.a, 0, 1));

                return float4(col, clamp(frame.a + 1 - node.a, 0, 1));
            }
            ENDCG
        }
    }
}
