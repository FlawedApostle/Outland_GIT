using UnityEngine;
using UnityEditor;

public class VHSInspector : ShaderGUI
{
    // Foldout states to keep the UI clean
    bool showLens = true;
    bool showChroma = false;
    bool showGlitch = true;
    bool showRGB = false;
    bool showBleed = false;
    bool showGrain = true;
    bool showFuzzy = false;

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
    {
        // --- 1. LENS & DISTORTION ---
        DrawSection("1. Lens & Distortion", ref showLens, () => {
            MaterialProperty fisheye = FindProperty("_UseFisheye", properties);
            materialEditor.ShaderProperty(fisheye, "Enable Lens FX");

            if (fisheye.floatValue == 1)
            {
                EditorGUI.indentLevel++;
                materialEditor.ShaderProperty(FindProperty("_DistortionStrength", properties), "Lens Bulge");
                materialEditor.ShaderProperty(FindProperty("_BlurStrength", properties), "Edge Blur Intensity");
                materialEditor.ShaderProperty(FindProperty("_Zoom", properties), "Zoom");
                EditorGUI.indentLevel--;
            }
        });

        // --- 2. CHROMATIC ABERRATION ---
        DrawSection("2. Chromatic Aberration", ref showChroma, () => {
            MaterialProperty chromaAbb = FindProperty("_UseChromaAbb", properties);
            materialEditor.ShaderProperty(chromaAbb, "Enable Lens Split");
            if (chromaAbb.floatValue == 1)
            {
                materialEditor.ShaderProperty(FindProperty("_AbbIntensity", properties), "Split Strength");
            }
        });

        // --- 3. TRACKING & DAMAGE ---
        DrawSection("3. Tracking & Damage", ref showGlitch, () => {
            MaterialProperty glitch = FindProperty("_UseGlitch", properties);
            materialEditor.ShaderProperty(glitch, "Enable Damage");
            if (glitch.floatValue == 1)
            {
                EditorGUI.indentLevel++;
                materialEditor.ShaderProperty(FindProperty("_TrackingSpeed", properties), "Glitch Scroll Speed");
                materialEditor.ShaderProperty(FindProperty("_TrackingSize", properties), "Glitch Band Size");
                EditorGUI.indentLevel--;
            }
            EditorGUILayout.Space();
            MaterialProperty blackout = FindProperty("_UseBlackout", properties);
            materialEditor.ShaderProperty(blackout, "Enable Random Blackout");
            if (blackout.floatValue == 1)
            {
                materialEditor.ShaderProperty(FindProperty("_CutoutThreshold", properties), "Blackout Chance");
            }
        });

        // --- 4. CONSTANT RGB SPLIT ---
        DrawSection("4. Constant RGB Split", ref showRGB, () => {
            MaterialProperty chroma = FindProperty("_UseChroma", properties);
            materialEditor.ShaderProperty(chroma, "Enable Constant Split");
            if (chroma.floatValue == 1)
            {
                materialEditor.ShaderProperty(FindProperty("_R_Offset", properties), "Red Offset");
                materialEditor.ShaderProperty(FindProperty("_G_Offset", properties), "Green Offset");
                materialEditor.ShaderProperty(FindProperty("_B_Offset", properties), "Blue Offset");
            }
        });

        // --- 5. COLOR BLEEDING ---
        DrawSection("5. Color Bleeding", ref showBleed, () => {
            MaterialProperty bleed = FindProperty("_UseBleed", properties);
            materialEditor.ShaderProperty(bleed, "Enable Color Bleed");
            if (bleed.floatValue == 1)
            {
                materialEditor.ShaderProperty(FindProperty("_BleedAmount", properties), "Bleed Range");
                materialEditor.ShaderProperty(FindProperty("_BleedR", properties), "Red Intensity");
                materialEditor.ShaderProperty(FindProperty("_BleedG", properties), "Green Intensity");
                materialEditor.ShaderProperty(FindProperty("_BleedB", properties), "Blue Intensity");
            }
        });

        // --- 6. STATIC AND LINES ---
        DrawSection("6. Static and Lines", ref showGrain, () =>
        {
            MaterialProperty grain = FindProperty("_UseGrain", properties);
            materialEditor.ShaderProperty(grain, "Enable BW Grain");

            if (grain.floatValue == 1)
            {
                materialEditor.ShaderProperty(
                    FindProperty("_GrainIntensity", properties),
                    "Static Amount");
            }

            EditorGUILayout.Space();

            MaterialProperty lines = FindProperty("_UseLines", properties);
            materialEditor.ShaderProperty(lines, "Enable Scanlines");

            if (lines.floatValue == 1)
            {
                materialEditor.ShaderProperty(
                    FindProperty("_LineDensity", properties),
                    "Line Density");

                materialEditor.ShaderProperty(
                    FindProperty("_LineSpeed", properties),
                    "Line Speed");

                materialEditor.ShaderProperty(
                    FindProperty("_LineStrength", properties),
                    "Line Strength");

                EditorGUILayout.Space();

                // Warp
                MaterialProperty warp = FindProperty("_UseWarp", properties);
                materialEditor.ShaderProperty(warp, "Enable Line Warp");

                if (warp.floatValue == 1)
                {
                    materialEditor.ShaderProperty(
                        FindProperty("_WarpStrength", properties),
                        "Warp Strength");

                    materialEditor.ShaderProperty(
                        FindProperty("_WarpSpeed", properties),
                        "Warp Speed");
                }
            }

            EditorGUILayout.Space();

            // Flicker explain
            MaterialProperty flicker = FindProperty("_UseFlicker", properties);
            materialEditor.ShaderProperty(flicker, "Enable Flicker");

            if (flicker.floatValue == 1)
            {
                materialEditor.ShaderProperty(
                    FindProperty("_FlickerStrength", properties),
                    "Flicker Strength");
            }

            EditorGUILayout.Space();

            // Vertical Jump
            MaterialProperty vJump = FindProperty("_UseVerticalJump", properties);
            materialEditor.ShaderProperty(vJump, "Enable Vertical Jump");

            if (vJump.floatValue == 1)
            {
                materialEditor.ShaderProperty(
                    FindProperty("_VerticalJumpStrength", properties),
                    "Jump Strength");
            }
        });


        // --- 6b. VERTICAL JUMP (rides lens distortion) ---
        DrawSection("6b. Vertical Jump", ref showGrain, () =>
        {
            MaterialProperty verticalJump = FindProperty("_UseVerticalJump", properties);
            materialEditor.ShaderProperty(verticalJump, "Enable Vertical Jump");

            if (verticalJump.floatValue == 1)
            {
                EditorGUI.indentLevel++;
                materialEditor.ShaderProperty(FindProperty("_VerticalJumpStrength", properties), "Jump Strength");

                // Add info label that Vertical Jump now rides lens distortion
                EditorGUILayout.HelpBox(
                    "Vertical Jump now rides Lens Distortion (fisheye), so the jump follows the lens curvature.",
                    MessageType.Info
                );

                EditorGUI.indentLevel--;
            }
        });



        /*
        DrawSection("6. Static and Lines", ref showGrain, () => {
            MaterialProperty grain = FindProperty("_UseGrain", properties);
            materialEditor.ShaderProperty(grain, "Enable BW Grain");
            if (grain.floatValue == 1)
            {
                materialEditor.ShaderProperty(FindProperty("_GrainIntensity", properties), "Static Amount");
            }
            EditorGUILayout.Space();
            MaterialProperty lines = FindProperty("_UseLines", properties);
            materialEditor.ShaderProperty(lines, "Enable Scanlines");
            if (lines.floatValue == 1)
            {
                materialEditor.ShaderProperty(FindProperty("_LineDensity", properties), "Line Density");
                materialEditor.ShaderProperty(FindProperty("_LineSpeed", properties), "Line Speed");
            }
        });

        */

        // --- 7. CHROMATIC COLOR GRAIN ---
        DrawSection("7. Chromatic Color Grain", ref showFuzzy, () => {
            MaterialProperty colorGrain = FindProperty("_UseColorGrain", properties);
            materialEditor.ShaderProperty(colorGrain, "Enable RGB Fuzzy Grain");
            if (colorGrain.floatValue == 1)
            {
                materialEditor.ShaderProperty(FindProperty("_ColorGrainIntensity", properties), "Overall Strength");
                materialEditor.ShaderProperty(FindProperty("_ColorGrainRGB", properties), "RGB Balance");
                materialEditor.ShaderProperty(FindProperty("_Chunkiness", properties), "Grain Chunkiness");
            }
        });

        EditorGUILayout.Space();
        materialEditor.RenderQueueField();
    }

    void DrawSection(string title, ref bool state, System.Action content)
    {
        state = EditorGUILayout.BeginFoldoutHeaderGroup(state, title);
        if (state)
        {
            EditorGUILayout.BeginVertical("Box");
            content.Invoke();
            EditorGUILayout.EndVertical();
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }
}