Shader "Hidden/FisheyeCode"
{
    Properties
    {
        _DistortionStrength("Distortion Strength", Float) = 0.5
        _BlurStrength("Blur Strength", Float) = 1.0
        _Zoom("Zoom (Overscan)", Float) = 0.9

        [Header(VHS Settings)]
        [Toggle(_USE_VHS_ON)] _UseVHS("Enable VHS Scanlines", Float) = 1
        _ScanlineCount("Scanline Density", Float) = 800
        _ScanlineSpeed("Roll Speed", Float) = 2.0
        _ScanlineIntensity("Scanline Strength", Range(0, 0.1)) = 0.04

        [Header (VHS Glitch)]
        [Toggle(_USE_GLITCH_ON)] _UseGlitch("Enable Jitter Glitch", Float) = 1
        _GlitchStrength("Glitch Intensity", Range(0, 0.1)) = 0.02
        _GlitchSpeed("Glitch Speed", Float) = 10.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off

        Pass
        {
            Name "FisheyePass"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag
            
            // This line tells Unity to listen to the Toggle checkbox
            #pragma shader_feature _USE_VHS_ON
            #pragma shader_feature _USE_VHS_GLITCH_ON

            float _DistortionStrength;
            float _BlurStrength;
            float _Zoom;
            
            float _ScanlineCount;
            float _ScanlineSpeed;
            float _ScanlineIntensity;

            float _GlitchStrength;
            float _GlitchSpeed;


            half4 Frag (Varyings input) : SV_Target
            {
                // 1. Get the UVs of the screen
                float2 uv = input.texcoord;
                // 2. Center the UVs (so 0,0 is the middle of the screen)
                float2 centeredUV = uv - 0.5;
                // 3. Get the distance from the center
                float dist = length(centeredUV);

                // 4. Warp + Zoom
                float2 distortedUV = 0.5 + (centeredUV * _Zoom) * (1.0 + _DistortionStrength * dist * dist);

                #ifdef _USE_VHS_GLITCH_ON
                // 4b. The Horizontal Glitch
                // We use a high-frequency sine wave mixed with time to create "random" jumps
                float glitchLine = sin(_Time.y * _GlitchSpeed + uv.y * 100.0);
                float glitchSqueeze = max(0, glitchLine - 0.9); // Only triggers when the wave is at its peak
                // Apply the "kick" to the horizontal (x) coordinate
                distortedUV.x += glitchSqueeze * _GlitchStrength;
                #endif

                // 5. The Edge Blur
                float blurAmount = dist * _BlurStrength * 0.005;
                half4 color = 0;

                color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, distortedUV + float2(blurAmount, 0));
                color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, distortedUV + float2(-blurAmount, 0));
                color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, distortedUV + float2(0, blurAmount));
                color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, distortedUV + float2(0, -blurAmount));
                color /= 4.0; 

                // 6. Rolling VHS Effect
                #ifdef _USE_VHS_ON
                    // uv.y * _ScanlineCount creates the lines
                    // _Time.y * _ScanlineSpeed makes them roll
                    float roll = (uv.y * _ScanlineCount) + (_Time.y * _ScanlineSpeed);
                    float scanline = sin(roll) * _ScanlineIntensity;
                    
                    color.rgb -= scanline;
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
        _DistortionStrength("Distortion Strength", Float) = 0.5
        _BlurStrength("Blur Strength", Float) = 1.0
        _Zoom("Zoom (Overscan)", Float) = 0.9
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off Cull Off

        Pass
        {
            Name "FisheyePass"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag

            // These must match the Properties names exactly
            float _DistortionStrength;
            float _BlurStrength;
            float _Zoom;

            half4 Frag (Varyings input) : SV_Target
            {
                // 1. Get the UVs of the screen
                float2 uv = input.texcoord;

                // 2. Center the UVs (so 0,0 is the middle of the screen)
                float2 centeredUV = uv - 0.5;

                // 3. Get the distance from the center
                float dist = length(centeredUV);

                // 4. Warp + Zoom
                // Using _Zoom here allows you to push the black edges off-screen
                float2 distortedUV = 0.5 + (centeredUV * _Zoom) * (1.0 + _DistortionStrength * dist * dist);

                // 5. The Edge Blur
                float blurAmount = dist * _BlurStrength * 0.005;
                half4 color = 0;

                // Sample in a diamond shape
                color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, distortedUV + float2(blurAmount, 0));
                color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, distortedUV + float2(-blurAmount, 0));
                color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, distortedUV + float2(0, blurAmount));
                color += SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_LinearClamp, distortedUV + float2(0, -blurAmount));
                
                color /= 4.0; // Average the 4 samples

                // Line 1: Create a repeating horizontal pattern based on screen UV
                float scanline = sin(uv.y * 800.0) * 0.04;
                
                // Line 2: Subtract that pattern from the color
                color.rgb -= scanline;

                // VHS "Crushed Color" look
                // color.rgb = floor(color.rgb * 10.0) / 10.0;

                return color;
            }
            ENDHLSL
        }
    }
}

                */