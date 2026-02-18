using UnityEngine;
using UnityEditor;

public class VHSInspector : ShaderGUI
{
    bool showLens = true; bool showChroma = true; bool showGlitch = true;
    bool showConstant = true; bool showBleed = true; bool showGrain = true; bool showFuzzy = true; bool showJitter = true;
    bool showVignette = true;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        // 1. LENS
        DrawSection("1. Lens & Distortion", ref showLens, () => {
            materialEditor.ShaderProperty(FindProperty("_UseFisheye", properties), "Enable Lens FX");
            materialEditor.ShaderProperty(FindProperty("_DistortionStrength", properties), "Lens Bulge");
            materialEditor.ShaderProperty(FindProperty("_BlurStrength", properties), "Edge Blur Intensity");
            materialEditor.ShaderProperty(FindProperty("_Zoom", properties), "Zoom");
            materialEditor.ShaderProperty(FindProperty("_DistortionPower", properties), "Distortion Power");
        });

        // 2. CHROMA ABB
        DrawSection("2. Chromatic Aberration", ref showChroma, () => {
            materialEditor.ShaderProperty(FindProperty("_UseChromaAbb", properties), "Enable Lens Split");
            materialEditor.ShaderProperty(FindProperty("_AbbIntensity", properties), "Edge Split Strength");
        });

        // 3. TRACKING
        DrawSection("3. Tracking & Damage", ref showGlitch, () => {
            materialEditor.ShaderProperty(FindProperty("_UseGlitch", properties), "Enable Damage");
            materialEditor.ShaderProperty(FindProperty("_TrackingSpeed", properties), "Scroll Speed");
            materialEditor.ShaderProperty(FindProperty("_TrackingAmount", properties), "Band Count (Lines)");
            materialEditor.ShaderProperty(FindProperty("_TrackingSpacing", properties), "Band Spacing (Delay)");

            materialEditor.ShaderProperty(FindProperty("_UseGlitchColor", properties), "Colorize Glitch Band");
            materialEditor.ShaderProperty(FindProperty("_GlitchRGB", properties), "Band RGB Color");

            EditorGUILayout.Space();
            materialEditor.ShaderProperty(FindProperty("_UseRGBBurst", properties), "Enable Color Bursts");
            materialEditor.ShaderProperty(FindProperty("_BurstScroll", properties), "Burst Scroll");
            materialEditor.ShaderProperty(FindProperty("_BurstSize", properties), "Burst Height");
            materialEditor.ShaderProperty(FindProperty("_BurstInterval", properties), "Burst Frequency");
            materialEditor.ShaderProperty(FindProperty("_BurstBrightness", properties), "Burst Intensity");
            materialEditor.ShaderProperty(FindProperty("_BurstColor", properties), "Burst RGB Color");

            materialEditor.ShaderProperty(FindProperty("_UseBlackout", properties), "Enable Blackout");
        });

        // 4. CONSTANT SPLIT
        DrawSection("4. Constant RGB Split", ref showConstant, () => {
            materialEditor.ShaderProperty(FindProperty("_UseChroma", properties), "Enable Constant Split");
            materialEditor.ShaderProperty(FindProperty("_R_Offset", properties), "Red Offset");
            materialEditor.ShaderProperty(FindProperty("_G_Offset", properties), "Green Offset");
            materialEditor.ShaderProperty(FindProperty("_B_Offset", properties), "Blue Offset");
        });

        // 5. BLEED
        DrawSection("5. Color Bleeding", ref showBleed, () => {
            materialEditor.ShaderProperty(FindProperty("_UseBleed", properties), "Enable Color Bleed");
            materialEditor.ShaderProperty(FindProperty("_BleedAmount", properties), "Bleed Range");
            materialEditor.ShaderProperty(FindProperty("_BleedR", properties), "Red Intensity");
            materialEditor.ShaderProperty(FindProperty("_BleedG", properties), "Green Intensity");
            materialEditor.ShaderProperty(FindProperty("_BleedB", properties), "Blue Intensity");
        });

        // 6. STATIC
        DrawSection("6. Static and Lines", ref showGrain, () => {
            materialEditor.ShaderProperty(FindProperty("_UseGrain", properties), "Enable BW Grain");
            materialEditor.ShaderProperty(FindProperty("_GrainIntensity", properties), "Grain Amount");
            materialEditor.ShaderProperty(FindProperty("_UseLines", properties), "Enable Scanlines");
            materialEditor.ShaderProperty(FindProperty("_LineDensity", properties), "Line Density");
            materialEditor.ShaderProperty(FindProperty("_LineSpeed", properties), "Line Speed");
            materialEditor.ShaderProperty(FindProperty("_LineStrength", properties), "Line Strength");
            materialEditor.ShaderProperty(FindProperty("_UseWarp", properties), "Enable Line Warp");
            materialEditor.ShaderProperty(FindProperty("_WarpStrength", properties), "Warp Strength");
            materialEditor.ShaderProperty(FindProperty("_UseFlicker", properties), "Enable Flicker");
            materialEditor.ShaderProperty(FindProperty("_UseVerticalJump", properties), "Enable Vertical Jump");
        });

        // 7. FUZZY
        DrawSection("7. Chromatic Color Grain", ref showFuzzy, () => {
            materialEditor.ShaderProperty(FindProperty("_UseColorGrain", properties), "Enable RGB Fuzzy Grain");
            materialEditor.ShaderProperty(FindProperty("_ColorGrainIntensity", properties), "Fuzzy Strength");
            materialEditor.ShaderProperty(FindProperty("_ColorGrainRGB", properties), "RGB Balance");
            materialEditor.ShaderProperty(FindProperty("_Chunkiness", properties), "Grain Chunkiness");
        });


        // 8. FRAME JITTER (NEW)
        DrawSection("8. Frame Jitter", ref showJitter, () => {
            materialEditor.ShaderProperty(FindProperty("_UseJitter", properties), "Enable Frame Jitter");
            materialEditor.ShaderProperty(FindProperty("_JitterAmount", properties), "Jitter Intensity");
            materialEditor.ShaderProperty(FindProperty("_JitterSpeed", properties), "Jitter Speed");
        });

        // 9. vignette 
        DrawSection("9. Vignette", ref showVignette, () =>
        {
            materialEditor.ShaderProperty(FindProperty("_UseVignette", properties), "Enable Vignette");
            materialEditor.ShaderProperty(FindProperty("_VignetteStrength", properties), "Edge Darkness");
            materialEditor.ShaderProperty(FindProperty("_VignetteSize", properties), " Vignette Smoothness (Power)");    

        });


        materialEditor.RenderQueueField();
    }

    void DrawSection(string title, ref bool state, System.Action content)
    {
        state = EditorGUILayout.BeginFoldoutHeaderGroup(state, title);
        if (state) { EditorGUILayout.BeginVertical("Box"); content.Invoke(); EditorGUILayout.EndVertical(); }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }
}