// ================================================================================
// COPYRIGHT (C) 2026 [Samuel Fearnley]. ALL RIGHTS RESERVED.
// This shader is provided for use in projects but may not be resold or 
// redistributed as source code without express permission.
// ================================================================================
// "RenderPipeline" = "UniversalPipeline": 
// This is a "lock and key" tag. It tells Unity, "Only run this shader if the project is using URP." 
// If you tried to use this in the old Built-in pipeline, it WONT'T run.
// ================================================================================

// Shader "Hidden/VHS_Final_Master"
Shader "VHS_Effects/VHS_Final_Master" // CHANGED FROM HIDDEN
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        
        [Header(1. Lens Distortion and Edge Blur)]
        [Toggle(_USE_FISHEYE_ON)] _UseFisheye("Enable Lens FX", Float) = 1
        _DistortionStrength("Lens Bulge", Float) = 0.5
        _BlurStrength("Edge Blur Intensity", Range(0, 5)) = 1.0
        _Zoom("Zoom", Float) = 0.9
        _DistortionPower("Lens Edge Sharpness", Range(1, 5)) = 2.0              // pow() formula

        [Header(2. Chromatic Aberration)]
        [Toggle(_USE_CHROMA_ABB)] _UseChromaAbb("Enable Lens Split", Float) = 0
        _AbbIntensity("Edge Split Strength", Range(0, 0.05)) = 0.01

        [Header(3. Tracking Glitch and Damage)]
        [Toggle(_USE_GLITCH_ON)] _UseGlitch("Enable Damage", Float) = 1
        _TrackingSpeed("Band Scroll Speed", Float) = 1.0
        _TrackingSize("Band Thickness", Range(0, 20)) = 10.0
        _TrackingAmount("Number of Bands", Range(1, 10)) = 1.0
        _TrackingSpacing("Band Spacing (Loop)", Range(0, 10)) = 1.0
        [Toggle(_USE_GLITCH_COLOR)] _UseGlitchColor("Colorize Glitch Band", Float) = 0
        _GlitchRGB("Glitch Band RGB", Vector) = (1,1,1,1)

        [Toggle(_USE_BLACKOUT)] _UseBlackout("Enable Random Blackout", Float) = 1
        _CutoutThreshold("Blackout Chance", Range(0.9, 1.0)) = 0.98
        
        // NEW FEATURE: RGB GLITCH BURSTS (Child of Glitch)
        [Toggle(_USE_RGB_BURST)] _UseRGBBurst("Enable Color Bursts", Float) = 0
        [Toggle(_USE_BURST_SCROLL)] _BurstScroll("Make Burst Scroll", Float) = 0
        _BurstSize("Burst Height", Range(0, 1)) = 0.1
        _BurstInterval("Burst Frequency", Range(0, 1)) = 0.95
        _BurstBrightness("Burst Intensity", Range(0, 2)) = 1.0
        _BurstColor("Burst RGB Color", Vector) = (1,1,1,1)

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
        _LineStrength("Scanline Strength", Range(0, 1)) = 0.1
        _LineRotate("Line Rotation", Range(0, 6.28)) = 0 
        _LineSineWarp("Line Sine Bend", Range(0, 0.1)) = 0

        [Toggle(_USE_WARP_ON)] _UseWarp("Enable Line Warp", Float) = 0
        _WarpStrength("Warp Strength", Range(0, 0.05)) = 0.01
        _WarpSpeed("Warp Speed", Float) = 1.0
        [Toggle(_USE_FLICKER_ON)] _UseFlicker("Enable Flicker", Float) = 0
        _FlickerStrength("Flicker Strength", Range(0, 0.2)) = 0.05
        _FlickerSpeed("Flicker Speed", Float) = 10.0
        [Toggle(_USE_VERTICAL_JUMP)] _UseVerticalJump("Enable Vertical Jump", Float) = 0
        _VerticalJumpStrength("Vertical Jump Strength", Range(0, 0.1)) = 0.02

        [Header(7. Chromatic Color Grain)]
        [Toggle(_USE_COLOR_GRAIN)] _UseColorGrain("Enable RGB Fuzzy Grain", Float) = 0
        _ColorGrainIntensity("Overall Fuzzy Strength", Range(0, 0.5)) = 0.1
        _ColorGrainRGB("RGB Balance (R, G, B)", Vector) = (1, 1, 1, 0)
        _Chunkiness("Grain Chunkiness", Range(1, 1000)) = 500

        [Header(8. Frame Jitter)]
        [Toggle(_USE_JITTER)] _UseJitter("Enable Frame Jitter", Float) = 0
        _JitterAmount("Jitter Intensity", Range(0, 0.01)) = 0.001
        _JitterSpeed("Jitter Speed", Float) = 20.0

        [Header(9. Border Vignette)]
        [Toggle(_USE_VIGNETTE)] _UseVignette("Enable Vignette", Float) = 0
        _VignetteStrength("Edge Darkness", Range(0, 2)) = 1.0
        _VignetteSize("Vignette Smoothness", Range(0.1, 5)) = 1.0
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

            #pragma shader_feature_local _USE_FISHEYE_ON
            #pragma shader_feature_local _USE_CHROMA_ABB
            #pragma shader_feature_local _USE_GLITCH_ON
            #pragma shader_feature_local _USE_GLITCH_COLOR
            #pragma shader_feature_local _USE_RGB_BURST
            #pragma shader_feature_local _USE_BURST_SCROLL
            #pragma shader_feature_local _USE_BLACKOUT
            #pragma shader_feature_local _USE_CHROMA
            #pragma shader_feature_local _USE_BLEED
            #pragma shader_feature_local _USE_GRAIN_ON
            #pragma shader_feature_local _USE_LINES_ON
            #pragma shader_feature_local _USE_WARP_ON
            #pragma shader_feature_local _USE_FLICKER_ON
            #pragma shader_feature_local _USE_VERTICAL_JUMP
            #pragma shader_feature_local _USE_COLOR_GRAIN
            #pragma shader_feature_local _USE_JITTER
            #pragma shader_feature_local _USE_VIGNETTE

            float _DistortionStrength, _DistortionPower, _BlurStrength, _Zoom, _AbbIntensity;                                       // FISH EYE BLUR & DISTORTION
            float _TrackingSpeed, _TrackingSize, _TrackingAmount, _TrackingSpacing, _CutoutThreshold;                               //
            float4 _GlitchRGB;                                                                                                      //
            float _BurstSize, _BurstInterval, _BurstBrightness;                                                                     //
            float4 _BurstColor;                                                                                                     //
            float _R_Offset, _G_Offset, _B_Offset;                                                                                  //
            float _BleedAmount, _BleedR, _BleedG, _BleedB;                                                                          //
            float _GrainIntensity, _LineDensity, _LineSpeed, _LineStrength;                                                         //
            float _LineRotate, _LineSineWarp;                                                                                       //
            float _ColorGrainIntensity, _Chunkiness;                                                                                //  COLOR GRAIN
            float4 _ColorGrainRGB;                                                                                                  //  
            float _WarpStrength, _WarpSpeed, _FlickerStrength, _FlickerSpeed, _VerticalJumpStrength , _JitterSpeed;                 //  WARP , FLICKER
            float _JitterAmount;                                                                                                    //  JITTER
            float _VignetteStrength, _VignetteSize;                                                                                 //  VIGNETTE

            float Noise(float2 uv) {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            half4 Frag(Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;
                float2 distortedUV = uv;
                float t_stable = frac(_Time.y);

                // 1. BLACKOUT
                #ifdef _USE_BLACKOUT
                if(Noise(float2(t_stable, 0)) > _CutoutThreshold) return half4(0,0,0,1);
                #endif

                // 2. FISHEYE
                #ifdef _USE_FISHEYE_ON
                float2 centeredUV = uv - 0.5;
                float dist = dot(centeredUV, centeredUV);
                //distortedUV = 0.5 + (centeredUV * _Zoom) * (1.0 + _DistortionStrength * dist);
                // Wrapping dist in pow() protects the center of the screen
                distortedUV = 0.5 + (centeredUV * _Zoom) * (1.0 + _DistortionStrength * pow(dist, _DistortionPower));
                #endif

                // 3. VERTICAL JUMP
                #ifdef _USE_VERTICAL_JUMP
                float jump = step(0.95, Noise(float2(t_stable * 0.5, 0))) * _VerticalJumpStrength;
                distortedUV.y += jump;
                #endif

                // 4. TRACKING GLITCH (LOOPING BANDS)
                float3 glitchAddColor = 0;
                #ifdef _USE_GLITCH_ON
                float wave = (distortedUV.y * _TrackingAmount) - (_Time.y * _TrackingSpeed);
                float trackingBar = sin(wave * _TrackingSpacing);
                trackingBar = smoothstep(0.9, 1.0, trackingBar);
                
                distortedUV.x += trackingBar * 0.03 * Noise(float2(t_stable, distortedUV.y));
                
                #ifdef _USE_GLITCH_COLOR
                float pNoise = Noise(distortedUV + t_stable);
                glitchAddColor = trackingBar * pNoise * _GlitchRGB.rgb;
                #endif
                #endif

                // 13. FRAME JITTER (Vertical/Horizontal micro-shake) - has to be above SAMPLE because SAMPLE is updating distortedUV .. aka uv
                #ifdef _USE_JITTER
                float jitterTime = floor(_Time.y * _JitterSpeed);
                distortedUV.x += (Noise(float2(jitterTime, 0)) - 0.5) * _JitterAmount;
                distortedUV.y += (Noise(float2(0, jitterTime)) - 0.5) * _JitterAmount;
                #endif

                // 5. CHANNELS (Restored CA and Constant Split)
                float2 r_uv = distortedUV; float2 g_uv = distortedUV; float2 b_uv = distortedUV;
                #ifdef _USE_CHROMA_ABB
                float2 abbDir = distortedUV - 0.5;
                r_uv += abbDir * _AbbIntensity; b_uv -= abbDir * _AbbIntensity;
                #endif
                #ifdef _USE_CHROMA
                r_uv.x += _R_Offset; g_uv.x += _G_Offset; b_uv.x += _B_Offset;
                #endif

                // 6. SAMPLE
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
                
                color.rgb += glitchAddColor;
                color.a = 1.0;

                // 7. RGB BURSTS (DO NOT TOUCH)
                #ifdef _USE_RGB_BURST
                float burstChance = Noise(float2(floor(_Time.y * 2.0), 0));
                if(burstChance > _BurstInterval) {
                    float burstY = (_USE_BURST_SCROLL) ? frac(_Time.y * 1.5) : Noise(float2(floor(_Time.y * 5.0), 1.1));
                    float burstMask = smoothstep(_BurstSize, 0.0, abs(uv.y - burstY));
                    float3 bCol = float3(Noise(uv + t_stable), Noise(uv + t_stable + 0.3), Noise(uv + t_stable + 0.6));
                    color.rgb += bCol * burstMask * _BurstBrightness * _BurstColor.rgb;
                }
                #endif

                // 8. COLOR BLEED
                #ifdef _USE_BLEED
                float2 bleedUV = distortedUV + float2(_BleedAmount, 0);
                half4 smearCol = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, bleedUV);
                color.r = lerp(color.r, max(color.r, smearCol.r), _BleedR);
                color.g = lerp(color.g, max(color.g, smearCol.g), _BleedG);
                color.b = lerp(color.b, max(color.b, smearCol.b), _BleedB);
                #endif

                // 9. CHROMATIC COLOR GRAIN (Restored Fuzzy)
                #ifdef _USE_COLOR_GRAIN
                float2 chunkyUV = floor(uv * _Chunkiness) / _Chunkiness;
                float rN = Noise(chunkyUV + t_stable + 0.11);
                float gN = Noise(chunkyUV + t_stable + 0.33);
                float bN = Noise(chunkyUV + t_stable + 0.55);
                float3 fuzzyNoise = (float3(rN, gN, bN) - 0.5) * _ColorGrainIntensity;
                color.rgb += fuzzyNoise * _ColorGrainRGB.rgb;
                #endif

                // 10. BW GRAIN
                #ifdef _USE_GRAIN_ON
                color.rgb += (Noise(uv + t_stable) - 0.5) * _GrainIntensity;
                #endif

                // 11. SCANLINES & WARP
                #ifdef _USE_LINES_ON
                float cosR = cos(_LineRotate); float sinR = sin(_LineRotate);
                float2 rotatedUV = float2(uv.x * cosR - uv.y * sinR, uv.x * sinR + uv.y * cosR);
                float lineWarp = sin(uv.x * 10.0 + _Time.y) * _LineSineWarp;
                float lines = sin((rotatedUV.y + lineWarp) * _LineDensity - _Time.y * _LineSpeed);
                #ifdef _USE_WARP_ON
                lines += sin(uv.y * 10 + _Time.y * _WarpSpeed) * _WarpStrength;
                #endif
                color.rgb -= smoothstep(0.8, 1.0, lines) * _LineStrength;
                #endif

                // 12. FLICKER
                #ifdef _USE_FLICKER_ON
                color.rgb += (Noise(float2(t_stable * _FlickerSpeed, 0)) - 0.5) * _FlickerStrength;
                #endif

                // 14. VIGNETTE
                #ifdef _USE_VIGNETTE
                float2 vignetteUV = uv - 0.5;
                float vDist = length(vignetteUV);
                float vMask = saturate(1.0 - vDist * _VignetteStrength / _VignetteSize);
                color.rgb *= vMask;
                #endif

                return color;
            }
            ENDHLSL
        }
    }
    CustomEditor "VHSInspector"
}