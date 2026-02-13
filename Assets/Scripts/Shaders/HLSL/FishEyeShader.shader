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

                // Optional: Uncomment the line below to add the VHS "Crushed Color" look
                // color.rgb = floor(color.rgb * 10.0) / 10.0;

                return color;
            }
            ENDHLSL
        }
    }
}