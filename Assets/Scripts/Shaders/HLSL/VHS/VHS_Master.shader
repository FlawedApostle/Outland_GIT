// THIS SCRIPT CONTAINS IMPORTANT NOTES DENOTED AT THE START IN CAPS - THEY ARE NOT TO REMOVED
Shader "Hidden/VHS_Final_Master"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        
        [Header(1. Lens Distortion and Edge Blur)]
        [Toggle(_USE_FISHEYE_ON)] _UseFisheye("Enable Lens FX", Float) = 1
        _DistortionStrength("Lens Bulge", Float) = 0.5
        _BlurStrength("Edge Blur Intensity", Range(0, 5)) = 1.0
        _Zoom("Zoom", Float) = 0.9

        [Header(2. Chromatic Aberration)]
        [Toggle(_USE_CHROMA_ABB)] _UseChromaAbb("Enable Lens Split", Float) = 0
        _AbbIntensity("Edge Split Strength", Range(0, 0.05)) = 0.01

        [Header(3. Tracking Glitch and Damage)]
        [Toggle(_USE_GLITCH_ON)] _UseGlitch("Enable Damage", Float) = 1
        _TrackingSpeed("Glitch Scroll Speed", Float) = 1.0
        _TrackingSize("Glitch Band Size", Range(0, 20)) = 10.0
        [Toggle(_USE_BLACKOUT)] _UseBlackout("Enable Random Blackout", Float) = 1
        _CutoutThreshold("Blackout Chance", Range(0.9, 1.0)) = 0.98

        [Header(4. Constant RGB Split)]
        [Toggle(_USE_CHROMA)] _UseChroma("Enable Constant Split", Float) = 0
        _R_Offset("Red Offset", Range(-0.05, 0.05)) = 0.005
        _G_Offset("Green Offset", Range(-0.05, 0.05)) = 0.0
        _B_Offset("Blue Offset", Range(-0.05, 0.05)) = -0.005

        [Header(5. Color Bleeding)]
        [Toggle(_USE_BLEED)] _UseBleed("Enable Color Bleed", Float) = 0
        _BleedAmount("Bleed Range", Range(0, 0.1)) = 0.02
        _BleedR("Red Intensity", Range(0, 1)) = 1.0
        _BleedG("Green Intensity", Range(0, 1)) = 0.0
        _BleedB("Blue Intensity", Range(0, 1)) = 0.5

        [Header(6. Static and Lines)]
        [Toggle(_USE_GRAIN_ON)] _UseGrain("Enable BW Grain", Float) = 1
        _GrainIntensity("Static Grain Amount", Range(0, 0.2)) = 0.05
        [Toggle(_USE_LINES_ON)] _UseLines("Enable Scanlines", Float) = 1
        _LineDensity("Line Density", Float) = 200
        _LineSpeed("Line Speed", Float) = 0.5

        [Header(7. Chromatic Color Grain)]
        [Toggle(_USE_COLOR_GRAIN)] _UseColorGrain("Enable RGB Fuzzy Grain", Float) = 0
        _ColorGrainIntensity("Overall Fuzzy Strength", Range(0, 0.5)) = 0.1
        _ColorGrainRGB("RGB Balance (R, G, B)", Vector) = (1, 1, 1, 0)
        _Chunkiness("Grain Chunkiness", Range(1, 1000)) = 500
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma shader_feature _USE_FISHEYE_ON
            #pragma shader_feature _USE_CHROMA_ABB
            #pragma shader_feature _USE_GLITCH_ON
            #pragma shader_feature _USE_BLACKOUT
            #pragma shader_feature _USE_CHROMA
            #pragma shader_feature _USE_BLEED
            #pragma shader_feature _USE_GRAIN_ON
            #pragma shader_feature _USE_LINES_ON
            #pragma shader_feature _USE_COLOR_GRAIN

            float _DistortionStrength, _BlurStrength, _Zoom, _AbbIntensity;
            float _TrackingSpeed, _TrackingSize, _CutoutThreshold;
            float _R_Offset, _G_Offset, _B_Offset;
            float _BleedAmount, _BleedR, _BleedG, _BleedB;
            float _GrainIntensity, _LineDensity, _LineSpeed;
            float _ColorGrainIntensity, _Chunkiness;
            float4 _ColorGrainRGB;

            float Noise(float2 uv) {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            half4 Frag(Varyings input) : SV_Target {
                float2 uv = input.texcoord;
                float2 distortedUV = uv;

                // 1. BLACKOUT
                #ifdef _USE_BLACKOUT
                    if(Noise(float2(_Time.y, 0)) > _CutoutThreshold) return half4(0,0,0,1);
                #endif

                // 2. FISHEYE
                #ifdef _USE_FISHEYE_ON
                    float2 centeredUV = uv - 0.5;
                    float dist = length(centeredUV);
                    distortedUV = 0.5 + (centeredUV * _Zoom) * (1.0 + _DistortionStrength * dist * dist);
                #endif

                // 3. TRACKING
                #ifdef _USE_GLITCH_ON
                    float t = _Time.y * _TrackingSpeed;
                    float trackingBar = sin(uv.y * _TrackingSize - t);
                    trackingBar = smoothstep(0.9, 1.0, trackingBar); 
                    distortedUV.x += trackingBar * 0.02 * Noise(float2(t, uv.y));
                #endif

                // 4. CHANNELS
                float2 r_uv = distortedUV;
                float2 g_uv = distortedUV;
                float2 b_uv = distortedUV;

                #ifdef _USE_CHROMA_ABB
                    float2 abbDir = distortedUV - 0.5;
                    r_uv += abbDir * _AbbIntensity;
                    b_uv -= abbDir * _AbbIntensity;
                #endif

                #ifdef _USE_CHROMA
                    r_uv.x += _R_Offset;
                    g_uv.x += _G_Offset;
                    b_uv.x += _B_Offset;
                #endif

                // 5. SAMPLING & RADIAL BLUR
                half4 color = 0;
                #ifdef _USE_FISHEYE_ON
                    float distForBlur = length(uv - 0.5);
                    float blur = distForBlur * _BlurStrength * 0.005;
                    float2 offsets[4] = { float2(blur, blur), float2(-blur, blur), float2(blur, -blur), float2(-blur, -blur) };
                    
                    for(int i = 0; i < 4; i++) {
                        color.r += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, r_uv + offsets[i]).r;
                        color.g += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, g_uv + offsets[i]).g;
                        color.b += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, b_uv + offsets[i]).b;
                    }
                    color /= 4.0;
                #else
                    color.r = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, r_uv).r;
                    color.g = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, g_uv).g;
                    color.b = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, b_uv).b;
                #endif
                color.a = 1.0;

                // 6. BLEED
                #ifdef _USE_BLEED
                    float2 bleedUV = distortedUV + float2(_BleedAmount, 0);
                    half4 smearCol = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, bleedUV);
                    color.r = lerp(color.r, max(color.r, smearCol.r), _BleedR);
                    color.g = lerp(color.g, max(color.g, smearCol.g), _BleedG);
                    color.b = lerp(color.b, max(color.b, smearCol.b), _BleedB);
                #endif

                // 7. COLOR GRAIN
                #ifdef _USE_COLOR_GRAIN
                    float2 chunkyUV = floor(uv * _Chunkiness) / _Chunkiness;
                    float rN = Noise(chunkyUV + _Time.y);
                    float gN = Noise(chunkyUV + _Time.y + 0.5);
                    float bN = Noise(chunkyUV + _Time.y + 1.0);
                    float3 fuzzyNoise = (float3(rN, gN, bN) - 0.5) * _ColorGrainIntensity;
                    color.rgb += fuzzyNoise * _ColorGrainRGB.rgb;
                #endif

                // 8. BW GRAIN
                #ifdef _USE_GRAIN_ON
                    color.rgb += (Noise(uv * _Time.y) - 0.5) * _GrainIntensity;
                #endif

                // 9. SCANLINES
                #ifdef _USE_LINES_ON
                    float lines = sin(uv.y * _LineDensity - _Time.y * _LineSpeed);
                    color.rgb -= smoothstep(0.8, 1.0, lines) * 0.1;
                #endif

                return color;
            }
            ENDHLSL
        }
    }
}