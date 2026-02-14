Shader "Custom/TriplanarLit"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _TextureScale("Texture Scale", Float) = 1.0
        _Sharpness("Blending Sharpness", Range(1, 64)) = 10
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}

        Pass
        {
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag

            struct Attributes {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings {
                float4 positionCS : SV_POSITION;
                float3 worldPos   : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            float4 _MainTex_ST; 
            float _TextureScale, _Sharpness;

            Varyings Vert (Attributes input) {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.worldPos = TransformObjectToWorld(input.positionOS.xyz);
                output.worldNormal = TransformObjectToWorldNormal(input.normalOS);
                return output;
            }

            half4 Frag (Varyings input) : SV_Target {
                // 1. Calculate Weights (Adding tiny offset to prevent black spots)
                float3 weights = pow(abs(input.worldNormal), _Sharpness);
                weights /= (weights.x + weights.y + weights.z + 0.0001);

                // 2. Manual Tiling/Offset math
                float2 uvX = (input.worldPos.zy * _TextureScale) * _MainTex_ST.xy + _MainTex_ST.zw;
                float2 uvY = (input.worldPos.xz * _TextureScale) * _MainTex_ST.xy + _MainTex_ST.zw;
                float2 uvZ = (input.worldPos.xy * _TextureScale) * _MainTex_ST.xy + _MainTex_ST.zw;

                // 3. Sample
                half4 colX = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvX);
                half4 colY = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvY);
                half4 colZ = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvZ);

                // 4. Combine
                return colX * weights.x + colY * weights.y + colZ * weights.z;
            }
            ENDHLSL
        }
    }
}