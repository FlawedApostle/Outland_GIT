Shader "Hidden/FisheyeCode"
{
    Properties
    {
        [Header(FishEye Settings)]
        [Toggle(_USE_FISHEYE_ON)] _UseFisheye("Enable FishEye", Float) = 1
        _DistortionStrength("Lens Bulge", Float) = 0.5
        _BlurStrength("Blur Strength", Float) = 1.0
        _Zoom("Zoom", Float) = 0.9
        
        [Header(Tracking Glitch)]
        [Toggle(_USE_GLITCH_ON)] _UseGlitch("Enable Damage", Float) = 1
        _TrackingSpeed("Glitch Scroll Speed", Float) = 1.0
        _TrackingSize("Glitch Band Size", Range(0, 20)) = 10.0
        _CutoutThreshold("Blackout Chance", Range(0.9, 1.0)) = 0.98

        [Header(Static and Grain)]
        [Toggle(_USE_GRAIN_ON)] _UseGrain("Enable Grain", Float) = 1
        _GrainIntensity("Static Grain Amount", Range(0, 0.2)) = 0.05

        [Header(Scanlines)]
        [Toggle(_USE_LINES_ON)] _UseLines("Enable Scanlines", Float) = 1
        _LineDensity("Line Density", Float) = 200
        _LineSpeed("Line Speed", Float) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        ZWrite Off Cull Off

        Pass
        {
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag
            
            #pragma shader_feature _USE_FISHEYE_ON
            #pragma shader_feature _USE_GLITCH_ON
            #pragma shader_feature _USE_GRAIN_ON
            #pragma shader_feature _USE_LINES_ON

            float _DistortionStrength, _BlurStrength, _Zoom;
            float _GrainIntensity, _TrackingSpeed, _TrackingSize, _CutoutThreshold;
            float _LineDensity, _LineSpeed;

            float SimpleNoise(float2 uv) {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            half4 Frag (Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;
                float2 distortedUV = uv; 

                // 1. Fisheye Distortion Logic
                #ifdef _USE_FISHEYE_ON
                    float2 centeredUV = uv - 0.5;
                    float dist = length(centeredUV);
                    distortedUV = 0.5 + (centeredUV * _Zoom) * (1.0 + _DistortionStrength * dist * dist);
                #endif

                // 2. Tracking Glitch Logic
                #ifdef _USE_GLITCH_ON
                    float time = _Time.y * _TrackingSpeed;
                    float trackingBar = sin(uv.y * _TrackingSize - time);
                    trackingBar = smoothstep(0.9, 1.0, trackingBar); 
                    
                    distortedUV.x += trackingBar * 0.02 * SimpleNoise(float2(time, uv.y));

                    if(SimpleNoise(float2(_Time.y, 0)) > _CutoutThreshold) return half4(0,0,0,1);
                #endif

                // 3. Optimized Sample and Sharpness Fix
                half4 color = 0;

                #ifdef _USE_FISHEYE_ON
                    // Blur only applies if FishEye is toggled ON
                    float distForBlur = length(uv - 0.5);
                    float blur = distForBlur * _BlurStrength * 0.005;
                    color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, distortedUV + float2(blur, 0));
                    color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, distortedUV + float2(-blur, 0));
                    color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, distortedUV + float2(0, blur));
                    color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, distortedUV + float2(0, -blur));
                    color /= 4.0;
                #else
                    // Perfectly sharp sampling when FishEye is toggled OFF
                    color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, distortedUV);
                #endif

                // 4. Static Grain
                #ifdef _USE_GRAIN_ON
                    float grain = SimpleNoise(uv * _Time.y) * _GrainIntensity;
                    color.rgb += grain;
                #endif

                // 5. Scrolling Scanlines
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


/*
Shader "Hidden/FisheyeCode"
{
    Properties
    {
        [Header(FishEye Settings)]
        [Toggle(_USE_FISHEYE_ON)] _UseFisheye("Enable FishEye", Float) = 1
        _DistortionStrength("Lens Bulge", Float) = 0.5
        _BlurStrength("Blur Strength", Float) = 1.0
        _Zoom("Zoom", Float) = 0.9
        
        [Header(Tracking Glitch)]
        [Toggle(_USE_GLITCH_ON)] _UseGlitch("Enable Damage", Float) = 1
        _TrackingSpeed("Glitch Scroll Speed", Float) = 1.0
        _TrackingSize("Glitch Band Size", Range(0, 20)) = 10.0
        _CutoutThreshold("Blackout Chance", Range(0.9, 1.0)) = 0.98

        [Header(Static and Grain)]
        [Toggle(_USE_GRAIN_ON)] _UseGrain("Enable Grain", Float) = 1
        _GrainIntensity("Static Grain Amount", Range(0, 0.2)) = 0.05

        [Header(Scanlines)]
        [Toggle(_USE_LINES_ON)] _UseLines("Enable Scanlines", Float) = 1
        _LineDensity("Line Density", Float) = 200
        _LineSpeed("Line Speed", Float) = 0.5
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        ZWrite Off Cull Off

        Pass
        {
            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag
            
            // --- THE HANDSHAKE (Must be at the top) ---
            #pragma shader_feature _USE_FISHEYE_ON
            #pragma shader_feature _USE_GLITCH_ON
            #pragma shader_feature _USE_GRAIN_ON
            #pragma shader_feature _USE_LINES_ON

            // --- VARIABLE DECLARATIONS ---
            float _DistortionStrength, _BlurStrength, _Zoom;
            float _GrainIntensity, _TrackingSpeed, _TrackingSize, _CutoutThreshold;
            float _LineDensity, _LineSpeed;

            float SimpleNoise(float2 uv) {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            half4 Frag (Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;
                float2 distortedUV = uv; 

                // 1. Fisheye Logic
                #ifdef _USE_FISHEYE_ON
                    float2 centeredUV = uv - 0.5;
                    float dist = length(centeredUV);
                    distortedUV = 0.5 + (centeredUV * _Zoom) * (1.0 + _DistortionStrength * dist * dist);
                #endif

                float distForBlur = length(uv - 0.5);

                // 2. Tracking Glitch
                #ifdef _USE_GLITCH_ON
                    float time = _Time.y * _TrackingSpeed;
                    float trackingBar = sin(uv.y * _TrackingSize - time);
                    trackingBar = smoothstep(0.9, 1.0, trackingBar); 
                    
                    distortedUV.x += trackingBar * 0.02 * SimpleNoise(float2(time, uv.y));

                    if(SimpleNoise(float2(_Time.y, 0)) > _CutoutThreshold) return half4(0,0,0,1);
                #endif

                // 3. Sample and Blur
                float blur = distForBlur * _BlurStrength * 0.005;
                half4 color = 0;
                color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, distortedUV + float2(blur, 0));
                color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, distortedUV + float2(-blur, 0));
                color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, distortedUV + float2(0, blur));
                color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, distortedUV + float2(0, -blur));
                color /= 4.0;

                // 4. Static Grain
                #ifdef _USE_GRAIN_ON
                    float grain = SimpleNoise(uv * _Time.y) * _GrainIntensity;
                    color.rgb += grain;
                #endif

                // 5. Scrolling Scanlines (THE ADDITION)
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

                    */