Shader "AvatarViewer/Blit" {
    Properties
    {
        _MainTex ("Texture", any) = "" {}
        _Scale ("Scale", float) = 1.0
    }
    SubShader {
        Pass {
            ZTest Always Cull Off ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 4.0
            #include "UnityCG.cginc"

            UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
            float4 _MainTex_ST;
            float4 _Color;
            float _Scale;

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
                if (_Scale > 1.0) {
                    o.texcoord = o.texcoord * float2(_Scale, 1.0) + float2((1.0 - _Scale) / 2.0, 0);
                } else {
                    o.texcoord = o.texcoord * float2(1.0, 1.0 / _Scale) - float2(0, (1.0 / _Scale - 1.0) * 0.5);
                }
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                bool2 a = i.texcoord > float2(1.0, 1.0);
                bool2 b = i.texcoord < float2(0.0, 0.0);

                if (any(bool2(any(a), any(b))))
                    return fixed4(0, 0, 0, 1);

                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                return UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.texcoord);
            }
            ENDCG

        }
    }
    Fallback Off
}
