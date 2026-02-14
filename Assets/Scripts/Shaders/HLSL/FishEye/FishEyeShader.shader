Shader "Hidden/FisheyeCode"
{
    Properties
    {
        _DistortionStrength("Lens Bulge", Float) = 0.5
        _BlurStrength("Blur Strength", Float) = 1.0
        _Zoom("Zoom", Float) = 0.9
        
        [Header(Tracking Glitch)]
        _TrackingSpeed("Glitch Scroll Speed", Float) = 1.0
        _TrackingSize("Glitch Band Size", Range(0, 20)) = 10.0
        
        [Header(Damage Settings)]
        [Toggle(_USE_GLITCH_ON)] _UseGlitch("Enable Damage", Float) = 1
        _CutoutThreshold("Blackout Chance", Range(0.9, 1.0)) = 0.98
       

        [Header(Static and Grain)]
        [Toggle(_USE_GRAIN_ON)] _UseGrain("Enable Grain", Float) = 1
        _GrainIntensity("Static Grain Amount", Range(0, 0.2)) = 0.05
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
            #pragma shader_feature _USE_GLITCH_ON
            #pragma shader_feature _USE_GRAIN_ON

            float _DistortionStrength, _BlurStrength, _Zoom;
            float _GrainIntensity, _TrackingSpeed, _TrackingSize, _CutoutThreshold;

            // A helper function to create "Random" static noise
            float SimpleNoise(float2 uv) {
                return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
            }

            half4 Frag (Varyings input) : SV_Target
            {
                float2 uv = input.texcoord;
                float2 centeredUV = uv - 0.5;
                float dist = length(centeredUV);

                // 1. Fisheye + Warp
                float2 distortedUV = 0.5 + (centeredUV * _Zoom) * (1.0 + _DistortionStrength * dist * dist);

                // 2. Tracking Glitch (The "thick band" that moves)
                #ifdef _USE_GLITCH_ON
                    float time = _Time.y * _TrackingSpeed;
                    // This creates a moving "bar" that repeats
                    float trackingBar = sin(uv.y * _TrackingSize - time);
                    trackingBar = smoothstep(0.9, 1.0, trackingBar); // Make it a sharp band
                    
                    // Jitter the screen ONLY where the bar is
                    distortedUV.x += trackingBar * 0.02 * SimpleNoise(float2(time, uv.y));

                    // Random Blackout
                    if(SimpleNoise(float2(_Time.y, 0)) > _CutoutThreshold) return half4(0,0,0,1);
                #endif

                // 3. Sample and Blur
                float blur = dist * _BlurStrength * 0.005;
                half4 color = 0;
                color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, distortedUV + float2(blur, 0));
                color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, distortedUV + float2(-blur, 0));
                color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, distortedUV + float2(0, blur));
                color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, distortedUV + float2(0, -blur));
                color /= 4.0;

                // 4. Add Static Grain
                // We use Time in the noise so the grain "flickers"
                #ifdef _USE_GRAIN_ON
                float grain = SimpleNoise(uv * _Time.y) * _GrainIntensity;
                color.rgb += grain;
                #endif

                return color;
            }
            ENDHLSL
        }
    }
}