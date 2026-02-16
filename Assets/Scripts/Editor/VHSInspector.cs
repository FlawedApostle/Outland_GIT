using UnityEngine;
using UnityEditor;

public class VHSInspector : ShaderGUI
{
    bool showLens = true; bool showChroma = false; bool showGlitch = true;
    bool showRGB = false; bool showBleed = false; bool showGrain = true; bool showFuzzy = false;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        // 1. LENS
        DrawSection("1. Lens & Distortion", ref showLens, () => {
            MaterialProperty fisheye = FindProperty("_UseFisheye", properties);
            materialEditor.ShaderProperty(fisheye, "Enable Lens FX");
            if (fisheye.floatValue == 1)
            {
                materialEditor.ShaderProperty(FindProperty("_DistortionStrength", properties), "Lens Bulge");
                materialEditor.ShaderProperty(FindProperty("_BlurStrength", properties), "Edge Blur Intensity");
                materialEditor.ShaderProperty(FindProperty("_Zoom", properties), "Zoom");
            }
        });

        // 3. TRACKING & DAMAGE
        DrawSection("3. Tracking & Damage", ref showGlitch, () => {
            MaterialProperty glitch = FindProperty("_UseGlitch", properties);
            materialEditor.ShaderProperty(glitch, "Enable Damage");
            if (glitch.floatValue == 1)
            {
                materialEditor.ShaderProperty(FindProperty("_TrackingSpeed", properties), "Glitch Scroll Speed");
                materialEditor.ShaderProperty(FindProperty("_TrackingSize", properties), "Glitch Band Size");

                EditorGUILayout.Space();
                // CHILD OF GLITCH: RGB BURST
                MaterialProperty burst = FindProperty("_UseRGBBurst", properties);
                materialEditor.ShaderProperty(burst, "Enable Color Bursts (Child)");
                if (burst.floatValue == 1)
                {
                    EditorGUI.indentLevel++;
                    materialEditor.ShaderProperty(FindProperty("_BurstSize", properties), "Burst Size");
                    materialEditor.ShaderProperty(FindProperty("_BurstInterval", properties), "Burst Delay (Chance)");
                    materialEditor.ShaderProperty(FindProperty("_BurstBrightness", properties), "Burst Brightness");
                    EditorGUI.indentLevel--;
                }
            }
            materialEditor.ShaderProperty(FindProperty("_UseBlackout", properties), "Enable Random Blackout");
        });

        // 6. SCANLINES & WARPING 
        DrawSection("6. Static and Lines", ref showGrain, () => {
            materialEditor.ShaderProperty(FindProperty("_UseGrain", properties), "Enable BW Grain");
            MaterialProperty lines = FindProperty("_UseLines", properties);
            materialEditor.ShaderProperty(lines, "Enable Scanlines");
            if (lines.floatValue == 1)
            {
                materialEditor.ShaderProperty(FindProperty("_LineDensity", properties), "Density");
                materialEditor.ShaderProperty(FindProperty("_LineRotate", properties), "Line Rotation (Vertical/Horizontal)");
                materialEditor.ShaderProperty(FindProperty("_LineSineWarp", properties), "Line Sine Bend");
                materialEditor.ShaderProperty(FindProperty("_LineStrength", properties), "Strength");
            }
            materialEditor.ShaderProperty(FindProperty("_UseWarp", properties), "Enable Line Warp");
            materialEditor.ShaderProperty(FindProperty("_UseFlicker", properties), "Enable Flicker");
            materialEditor.ShaderProperty(FindProperty("_UseVerticalJump", properties), "Enable Vertical Jump");
        });

        // 7. CHROMATIC COLOR GRAIN
        DrawSection("7. Chromatic Color Grain", ref showFuzzy, () => {
            MaterialProperty fuzzy = FindProperty("_UseColorGrain", properties);
            materialEditor.ShaderProperty(fuzzy, "Enable RGB Fuzzy Grain");
            if (fuzzy.floatValue == 1)
            {
                materialEditor.ShaderProperty(FindProperty("_ColorGrainIntensity", properties), "Fuzzy Strength");
                materialEditor.ShaderProperty(FindProperty("_ColorGrainRGB", properties), "RGB Sliders");
                materialEditor.ShaderProperty(FindProperty("_Chunkiness", properties), "Chunkiness");
            }
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