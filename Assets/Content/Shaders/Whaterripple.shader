Shader "Custom/WaterRipple"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _RippleSpeed ("Ripple Speed", Range(0, 5)) = 1.0
        _RippleStrength ("Ripple Strength", Range(0, 0.1)) = 0.02
        _Frequency ("Wave Frequency", Range(5, 50)) = 20
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float _RippleSpeed;
            float _RippleStrength;
            float _Frequency;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);

                // Генерируем волновые искажения
                float2 wave = float2(sin(_Time.y * _RippleSpeed + v.uv.y * _Frequency),
                                     cos(_Time.y * _RippleSpeed + v.uv.x * _Frequency));

                o.uv = v.uv + wave * _RippleStrength;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
}