Shader "Unlit/Gradiant"
{
    Properties
    {
        _ColorA("Color A", Color) = (1,1,1,1)
        _ColorB("Color B", Color) = (0,0,0,1)
        _Weight("Color Weight", Range(-1, 1)) = 0
        [Toggle] _Flip("Flip", Integer) = 0
    }
    SubShader
    {
        ZTest Off
        Blend SrcAlpha OneMinusSrcAlpha
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

            float4 _ColorA;
            float4 _ColorB;
            float _Weight;
            float _Flip;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float easeInOutSine(float t)
            {
                return -(cos(UNITY_PI * t) - 1) / 2;
            }
            
            fixed4 frag(v2f i) : SV_Target
            {
                if (_Flip == 0)
                {
                    float e = easeInOutSine(i.uv.x);
                    fixed4 col = lerp(_ColorA, _ColorB, e - _Weight * (1 - e));
                    return col; 
                }
                
                float e = easeInOutSine(i.uv.y);
                fixed4 col = lerp(_ColorA, _ColorB, e - _Weight * (1 - e));
                return col;
            }
            ENDCG
        }
    }
}