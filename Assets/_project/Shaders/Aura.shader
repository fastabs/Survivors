Shader "Custom/Aura"
{
    Properties
    {
        _Color ("Base Color", Color) = (0.1, 0.2, 0.6, 0.4)
        _EdgeColor ("Edge Color", Color) = (0.2, 0.3, 1, 0.8)
        _Radius ("Radius", Float) = 1
        _EdgeWidth ("Edge Width", Float) = 0.25
        _NoiseScale ("Noise Scale", Float) = 6
        _NoiseStrength ("Noise Strength", Float) = 0.15
        _TimeScale ("Time Scale", Float) = 1
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
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
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float4 _Color;
            float4 _EdgeColor;
            float _Radius;
            float _EdgeWidth;
            float _NoiseScale;
            float _NoiseStrength;
            float _TimeScale;

            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(12.9898,78.233))) * 43758.5453);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float a = hash(i);
                float b = hash(i + float2(1,0));
                float c = hash(i + float2(0,1));
                float d = hash(i + float2(1,1));
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(a, b, u.x) +
                       (c - a)* u.y * (1.0 - u.x) +
                       (d - b) * u.x * u.y;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * 2 - 1;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float dist = length(i.uv);
                if (dist > 1) discard;

                float n = noise(i.uv * _NoiseScale + _Time.y * _TimeScale);
                dist += (n - 0.5) * _NoiseStrength;

                float edge = smoothstep(1 - _EdgeWidth, 1, dist);
                float alpha = smoothstep(1, 0, dist);

                float4 col = lerp(_Color, _EdgeColor, edge);
                col.a *= alpha;

                return col;
            }
            ENDHLSL
        }
    }
}
