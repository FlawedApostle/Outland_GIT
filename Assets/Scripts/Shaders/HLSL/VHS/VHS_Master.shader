// ================================================================================
// COPYRIGHT (C) 2026 []. ALL RIGHTS RESERVED.
// This shader is provided for use in projects but may not be resold or 
// redistributed as source code without express permission.
// ================================================================================
// "RenderPipeline" = "UniversalPipeline": 
// This is a "lock and key" tag. It tells Unity, "Only run this shader if the project is using URP." 
// If you tried to use this in the old Built-in pipeline, it WONT'T run.
// ================================================================================


// =========================================NOTES==================================
// [NOTE: FISH EYE - BLUR:      NEED TO ADD:    For BLUR CAN I CONTROL THE RADIUS OF DISTANCE FROM CENTER TO CORNER OR VICE VERSA ?] 
// [NOTE: SCANLINES - WARP:     NEED TO ADD:    Horizontal and vertical scan lines, warp on both x,y axis]
// [NOTE: BLACKOUT & FLICKER:   NEED TO ADD:    Float value to control timer, speed of flicker]
// [NOTE: 16 BIT ARCHITECTURE:  NEED TO FIX:    IM AWARE CBUFFER IS NOT IDEAL, IT IS PARTIALLY IMPLEMENTED. THE INJECTOR NEEDS TO ACCOMODATE - PENDING COMMANDS
// =========================================END OF NOTES============================

// Shader "Hidden/VHS_Final_Master"
Shader "VHS_Effects/VHS_Final_Master" // CHANGED FROM HIDDEN
{
    Properties
    {
         // BLACKOUT
        [Toggle(_USE_BLACKOUT)] _UseBlackout("Enable Random Blackout", Float) = 1
        _CutoutThreshold("Blackout Chance", Range(0.9, 1.0)) = 0.98  
        _MainTex("Texture", 2D) = "white" {}
        // LENS - FISH EYE   
        [Header(Lens Distortion and Edge Blur)]
        [Toggle(_USE_FISHEYE_ON)] _UseFisheye("Enable Lens FX", Float) = 1
        _DistortionStrength("Lens Bulge", Float) = 0.5
        _BlurStrength("Edge Blur Intensity", Range(0, 5)) = 1.0
        _Zoom("Zoom", Float) = 0.9
        _DistortionPower("Lens Edge Sharpness", Range(1, 5)) = 2.0              // pow() formula
        /// VIGNETTE BORDER
        [Header(Border Vignette)]
        [Toggle(_USE_VIGNETTE)] _UseVignette("Enable Vignette", Float) = 0
        _VignetteStrength("Edge Darkness", Range(0, 2)) = 1.0
        _VignetteSize("Vignette Smoothness", Range(0.1, 5)) = 1.0
        // CHROMATIC ABBERRATION
        [Header(Chromatic Aberration and Flare)]
        [Toggle(_USE_CHROMA_ABB)] _UseChromaAbb("Enable Radial Flare", Float) = 0
        _AbbIntensity("Flare Strength", Range(0, 0.5)) = 0.1
        _FlarePower("Flare Sharpness (Pow)", Range(1, 10)) = 3.0
        // CHROMATIC ABBERRATION - RGB SPLITTING
        [Header(Constant RGB Split)]
        [Toggle(_USE_CHROMA)] _UseChroma("Enable Constant Split", Float) = 0
        _R_Offset("Red Offset", Range(-0.05, 0.05)) = 0.005
        _G_Offset("Green Offset", Range(-0.05, 0.05)) = 0.0
        _B_Offset("Blue Offset", Range(-0.05, 0.05)) = -0.005
        // COLOR BLEEDING
        [Header(Color Bleeding)]
        [Toggle(_USE_BLEED)] _UseBleed("Enable Color Bleed", Float) = 0
        _BleedAmount("Bleed Range", Range(0, 0.1)) = 0.02
        _BleedR("Red Intensity", Range(0, 1)) = 1.0
        _BleedG("Green Intensity", Range(0, 1)) = 0.0
        _BleedB("Blue Intensity", Range(0, 1)) = 0.5
        // GLITCH 'HEAD-CLOGGING'
        [Header(Glitch Band)]
        [Toggle(_USE_GLITCH_ON)] _UseGlitch("Enable Damage", Float) = 1
        _TrackingSpeed("Band Scroll Speed", Float) = 1.0
        _TrackingSize("Band Thickness", Range(0, 20)) = 10.0
        _TrackingAmount("Number of Bands", Range(1, 10)) = 1.0
        _TrackingSpacing("Band Spacing (Loop)", Range(0, 10)) = 1.0
        [Toggle(_USE_GLITCH_COLOR)] _UseGlitchColor("Colorize Glitch Band", Float) = 0
        _GlitchRGB("Glitch Band RGB", Vector) = (1,1,1,1)
        // GLITCH CHILD - RGB GLITCH BURSTS
        [Toggle(_USE_RGB_BURST)] _UseRGBBurst("Enable Color Bursts", Float) = 0
        [Toggle(_USE_BURST_SCROLL)] _BurstScroll("Make Burst Scroll", Float) = 0
        _BurstSize("Burst Height", Range(0, 1)) = 0.1
        _BurstInterval("Burst Frequency", Range(0, 1)) = 0.95
        _BurstBrightness("Burst Intensity", Range(0, 2)) = 1.0
        _BurstColor("Burst RGB Color", Vector) = (1,1,1,1)

        // GRAIN - BW
        [Header(Static Grain)]
        [Toggle(_USE_GRAIN_ON)] _UseGrain("Enable BW Grain", Float) = 1
        _GrainIntensity("Static Grain Amount", Range(0, 0.2)) = 0.05
        /// GRAIN - COLOR CHROMATIC
        [Header(Chromatic Color Grain)]
        [Toggle(_USE_COLOR_GRAIN)] _UseColorGrain("Enable RGB Fuzzy Grain", Float) = 0
        _ColorGrainIntensity("Overall Fuzzy Strength", Range(0, 0.5)) = 0.1
        _ColorGrainRGB("RGB Balance (R, G, B)", Vector) = (1, 1, 1, 0)
        _Chunkiness("Grain Chunkiness", Range(1, 1000)) = 500
        
        // SCANLINES
        [Header(Scanlines)]
        [Toggle(_USE_LINES_ON)] _UseLines("Enable Scanlines", Float) = 1
        _LineDensity("Line Density", Float) = 200
        _LineSpeed("Line Speed", Float) = 0.5
        _LineStrength("Scanline Strength", Range(0, 1)) = 0.1
        _LineRotate("Line Rotation", Range(0, 6.28)) = 0 
        _LineSineWarp("Line Sine Bend", Range(0, 0.1)) = 0
        // SCANLINES - WARPING
        [Toggle(_USE_WARP_ON)] _UseWarp("Enable Line Warp", Float) = 0
        _WarpStrength("Warp Strength", Range(0, 0.05)) = 0.01
        _WarpSpeed("Warp Speed", Float) = 1.0
        
        // FLICKER - SCREEN (thinkin flicka)
        [Toggle(_USE_FLICKER_ON)] _UseFlicker("Enable Flicker", Float) = 0
        _FlickerStrength("Flicker Strength", Range(0, 0.2)) = 0.05
        _FlickerSpeed("Flicker Speed", Float) = 10.0
        ///VERTICAL JUMP - [FRAME JUMP - NOT JITTTER !]
        [Toggle(_USE_VERTICAL_JUMP)] _UseVerticalJump("Enable Vertical Jump", Float) = 0
        _VerticalJumpStrength("Vertical Jump Strength", Range(0, 0.1)) = 0.02

         ///FRAME JITTER - [FRAME JITTER- NOT JUMP!]
        [Header(Frame Jitter)]
        [Toggle(_USE_JITTER)] _UseJitter("Enable Frame Jitter", Float) = 0
        _JitterAmount("Jitter Intensity", Range(0, 0.01)) = 0.001
        _JitterSpeed("Jitter Speed", Float) = 20.0


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

            // 16 byte Architecture
             float4
             _GlitchRGB,
             _BurstColor,
             _ColorGrainRGB;                         // END OF FLOAT4  


            // GPU BUFFER
            CBUFFER_START(UnityPerMaterial)         // START OF CBUFFER     -- inside are the most used - i know it is not ideal
            // float4 _LensSettings;
            
            float 
            // LENS
            _DistortionStrength,
            _DistortionPower, 
            _BlurStrength, 
            _Zoom,
            // GLITCH
            _TrackingSpeed, 
            _TrackingAmount, 
            _TrackingSpacing,
            // COLOR
            _BleedAmount, 
            _BleedR, 
            _BleedG, 
            _BleedB,
            // GRAIN
            _GrainIntensity, 
            // VIGNETTE
            _VignetteStrength,
            _VignetteSize;                          // END OF FLOAT
            
            CBUFFER_END                             // END OF CBUFFER
            

            float
            _AbbIntensity, 
            _FlarePower,        
            _TrackingSize, 
            _CutoutThreshold,             
            _BurstSize, 
            _BurstInterval, 
            _BurstBrightness, 
            _R_Offset, 
            _G_Offset, 
            _B_Offset,
            _LineDensity, 
            _LineSpeed,
            _LineStrength,
            _LineRotate, 
            _LineSineWarp,
            _ColorGrainIntensity, 
            _Chunkiness,
            _WarpStrength, 
            _WarpSpeed, 
            _FlickerStrength, 
            _FlickerSpeed, 
            _VerticalJumpStrength, 
            _JitterSpeed,
            _JitterAmount;                          // END OF FLOAT


            float Noise(float2 uv) {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }
            /// test comment
            half4 Frag(Varyings input) : SV_Target
            {

                //--------------------------------------------------
                // STAGE 0: CBUFFER 16 BIT Architecture
                //--------------------------------------------------
                // 1. UNPACKING - 
                // We create "Local" variables inside the function.
                // These names only exist while the GPU is calculating this pixel.
                // float _DistortionStrength = _LensSettings.x;
                // float _DistortionPower    = _LensSettings.y;
                // float _BlurStrength       = _LensSettings.z;
                // float _Zoom               = _LensSettings.w;
                // uv = (uv - 0.5) * _Zoom + 0.5;
            

                //--------------------------------------------------
                // STAGE 0: INITIAL SETUP
                //--------------------------------------------------
                
                float2 uv = input.texcoord;

                // Effects
                float2 distortedUV = uv;
                float t_stable = frac(_Time.y);

                // -------> Distance Calculation (all features can use it, so it's outside the Fisheye block)
                float2 centeredUV = uv - 0.5;
                float dist = dot(centeredUV, centeredUV);

                half4 color = 0;            // -- BLUR
                float3 glitchAddColor = 0;  // GLITCH


                //--------------------------------------------------
                // STAGE 1: FRAME EVENTS
                //--------------------------------------------------
                
                 // BLACKOUT
                #ifdef _USE_BLACKOUT
                if(Noise(float2(t_stable, 0)) > _CutoutThreshold) return half4(0,0,0,1);
                #endif

                //--------------------------------------------------
                // STAGE 2: UV DISTORTION PIPELINE
                // These modify WHERE we sample from
                //--------------------------------------------------

                 // FISHEYE
                 #ifdef _USE_FISHEYE_ON
                // //distortedUV = 0.5 + (centeredUV * _Zoom) * (1.0 + _DistortionStrength * dist);
                // // Wrapping dist in pow() protects the center of the screen
                distortedUV = 0.5 + (centeredUV * _Zoom) * (1.0 + _DistortionStrength * pow(dist, _DistortionPower));
                #endif

                // VERTICAL TAPE JUMP
                #ifdef _USE_VERTICAL_JUMP
                float jump = step(0.95, Noise(float2(t_stable * 0.5, 0))) * _VerticalJumpStrength;
                distortedUV.y += jump;
                #endif
                // GLITCH - (LOOPING BANDS)
                #ifdef _USE_GLITCH_ON
                float wave = (distortedUV.y * _TrackingAmount) - (_Time.y * _TrackingSpeed);
                float trackingBar = sin(wave * _TrackingSpacing);
                trackingBar = smoothstep(0.9, 1.0, trackingBar);
                distortedUV.x += trackingBar * 0.03 * Noise(float2(t_stable, distortedUV.y));           // DSITORTED uv
                // GLITCH - COLOR
                #ifdef _USE_GLITCH_COLOR                                                                
                float pNoise = Noise(distortedUV + t_stable);
                glitchAddColor = trackingBar * pNoise * _GlitchRGB.rgb;
                #endif
                #endif

                //  FRAME JITTER (Vertical/Horizontal micro-shake) - has to be above SAMPLE because SAMPLE is updating distortedUV .. aka uv
                #ifdef _USE_JITTER
                float jitterTime = floor(_Time.y * _JitterSpeed);
                distortedUV.x += (Noise(float2(jitterTime, 0)) - 0.5) * _JitterAmount;
                distortedUV.y += (Noise(float2(0, jitterTime)) - 0.5) * _JitterAmount;
                #endif

                //--------------------------------------------------
                // STAGE 3: CHANNEL UV CALCULATION
                // These define RGB separation BEFORE sampling
                //--------------------------------------------------
                
                // CHANNELS (Flare vs Shift)
                float2 flareUV = distortedUV - 0.5;
                float flareFactor = 0; // Start at zero (no flare)
                
                // CHROMATIC ABBERATION - FLARE
                #ifdef _USE_CHROMA_ABB
                // We calculate the flare ONLY if the toggle is on
                flareFactor = _AbbIntensity * pow(dist, _FlarePower);
                #endif
                // Apply the Flare (or 0) to the Red and Blue channels
                float2 r_uv = 0.5 + flareUV * (1.0 + flareFactor);
                float2 g_uv = distortedUV;
                float2 b_uv = 0.5 + flareUV * (1.0 - flareFactor);
                // CHROMATIC ABBERATION - THE CONSTANT SHIFT (The Add-on Toggle)
                #ifdef _USE_CHROMA
                r_uv.x += _R_Offset;
                g_uv.x += _G_Offset;
                b_uv.x += _B_Offset;
                #endif
                
                //--------------------------------------------------
                // STAGE 4: IMAGE SAMPLING
                // This actually creates the image
                //--------------------------------------------------
                
                //  BLUR - SAMPLE 
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

                //--------------------------------------------------
                // STAGE 5: POST COLOR EFFECTS
                // These modify the sampled image
                //--------------------------------------------------

                //  RGB BURSTS (DO NOT TOUCH)
                #ifdef _USE_RGB_BURST
                float burstChance = Noise(float2(floor(_Time.y * 2.0), 0));
                if(burstChance > _BurstInterval) {
                    float burstY = (_USE_BURST_SCROLL) ? frac(_Time.y * 1.5) : Noise(float2(floor(_Time.y * 5.0), 1.1));
                    float burstMask = smoothstep(_BurstSize, 0.0, abs(uv.y - burstY));
                    float3 bCol = float3(Noise(uv + t_stable), Noise(uv + t_stable + 0.3), Noise(uv + t_stable + 0.6));
                    color.rgb += bCol * burstMask * _BurstBrightness * _BurstColor.rgb;
                }
                #endif

                //  COLOR BLEED - (Improved "Tape Lag")
                #ifdef _USE_BLEED
                // We sample slightly to the LEFT to make color "smear" to the RIGHT
                float2 bleedUV = distortedUV - float2(_BleedAmount, 0); 
                half4 smearCol = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, bleedUV);
                
                // Instead of just 'max', use a lerp to 'soften' the bleed
                color.r = lerp(color.r, smearCol.r, _BleedR);
                color.g = lerp(color.g, smearCol.g, _BleedG);
                color.b = lerp(color.b, smearCol.b, _BleedB);
                #endif

                // COLOR GRAIN - CHROMATIC COLOR (Restored Fuzzy)
                #ifdef _USE_COLOR_GRAIN
                float2 chunkyUV = floor(uv * _Chunkiness) / _Chunkiness;
                float rN = Noise(chunkyUV + t_stable + 0.11);
                float gN = Noise(chunkyUV + t_stable + 0.33);
                float bN = Noise(chunkyUV + t_stable + 0.55);
                float3 fuzzyNoise = (float3(rN, gN, bN) - 0.5) * _ColorGrainIntensity;
                color.rgb += fuzzyNoise * _ColorGrainRGB.rgb;
                #endif

                // COLOR GRAIN - BLACK AND WHITE
                #ifdef _USE_GRAIN_ON
                color.rgb += (Noise(uv + t_stable) - 0.5) * _GrainIntensity;
                #endif

                //  SCANLINES & WARP
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

                // FLICKER
                #ifdef _USE_FLICKER_ON
                color.rgb += (Noise(float2(t_stable * _FlickerSpeed, 0)) - 0.5) * _FlickerStrength;
                #endif

                // VIGNETTE
                #ifdef _USE_VIGNETTE
                float2 vignetteUV = uv - 0.5;
                float vDist = length(vignetteUV);
                float vMask = saturate(1.0 - vDist * _VignetteStrength / _VignetteSize);
                color.rgb *= vMask;
                #endif

                //--------------------------------------------------
                // FINAL OUTPUT
                //--------------------------------------------------
                // color = tex2D(_MainTex, uv);   GPU CBUFFER
                return color;
            }
            ENDHLSL
        }
    }
    CustomEditor "VHSInspector"
}