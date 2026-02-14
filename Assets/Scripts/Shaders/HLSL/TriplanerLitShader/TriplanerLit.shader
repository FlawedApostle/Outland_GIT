Shader "Custom/TriplanarLit_PBR"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _TextureScale("Texture Scale", Float) = 1.0
        _Sharpness("Blending Sharpness", Range(1, 64)) = 10
        
        [Header(Lighting Settings)]
        _Smoothness("Smoothness (Wetness)", Range(0, 1)) = 0.5
        _Metallic("Metallic", Range(0, 1)) = 0.0
        _BaseColorTint("Color Tint", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

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
            float _TextureScale, _Sharpness, _Smoothness, _Metallic;
            float4 _BaseColorTint;

            Varyings Vert (Attributes input) {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.worldPos = TransformObjectToWorld(input.positionOS.xyz);
                output.worldNormal = TransformObjectToWorldNormal(input.normalOS);
                return output;
            }

            half4 Frag (Varyings input) : SV_Target {
                // 1. Triplanar Math
                float3 weights = pow(abs(input.worldNormal), _Sharpness);
                weights /= (weights.x + weights.y + weights.z + 0.0001);

                float2 uvX = (input.worldPos.zy * _TextureScale) * _MainTex_ST.xy + _MainTex_ST.zw;
                float2 uvY = (input.worldPos.xz * _TextureScale) * _MainTex_ST.xy + _MainTex_ST.zw;
                float2 uvZ = (input.worldPos.xy * _TextureScale) * _MainTex_ST.xy + _MainTex_ST.zw;

                half3 albedo = (SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvX).rgb * weights.x +
                                SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvY).rgb * weights.y +
                                SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uvZ).rgb * weights.z) * _BaseColorTint.rgb;

                // 2. Lighting Setup (Manual - avoids the 'normalWorld' error)
                float3 normalWS = normalize(input.worldNormal);
                float3 viewDirWS = normalize(GetWorldSpaceViewDir(input.worldPos));
                
                // Get the main light (Sun)
                float4 shadowCoord = TransformWorldToShadowCoord(input.worldPos);
                Light mainLight = GetMainLight(shadowCoord);

                // 3. Simple Physically Based Lighting (PBR)
                // This combines the Albedo with the Light and adds the "Wetness" (Specular)
                half3 radiance = mainLight.color * (mainLight.distanceAttenuation * mainLight.shadowAttenuation);
                
                // Diffuse (The base color in light)
                half diffuseTerm = saturate(dot(normalWS, mainLight.direction));
                half3 diffuseColor = albedo * diffuseTerm;

                // Specular (The "Wet" shine)
                float3 halfDir = normalize(mainLight.direction + viewDirWS);
                float specTerm = pow(saturate(dot(normalWS, halfDir)), _Smoothness * 128.0);
                half3 specularColor = _Smoothness * specTerm * mainLight.color;

                // 4. Final Combination + Ambient
                half3 ambient = SampleSH(normalWS) * albedo;
                half3 finalRGB = (diffuseColor + specularColor) * radiance + ambient;

                return half4(finalRGB, 1.0);
            }
            ENDHLSL
        }
    }
}