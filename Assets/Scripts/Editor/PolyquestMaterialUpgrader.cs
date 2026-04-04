using UnityEditor;
using UnityEngine;
using System.IO;

public class PolyquestMaterialUpgrader
{
    private const string PolyquestMaterialsPath = "Assets/Polyquest Worlds Full Pack Vol.1/materials";

    [MenuItem("Tools/Upgrade Polyquest Materials to URP")]
    public static void UpgradeMaterials()
    {
        var shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null)
        {
            Debug.LogError("URP/Lit shader not found. Make sure URP is installed.");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:Material", new[] { PolyquestMaterialsPath });
        int upgraded = 0;
        int skipped = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null) continue;

            // Skip materials already on a URP shader
            if (mat.shader.name.StartsWith("Universal Render Pipeline"))
            {
                skipped++;
                continue;
            }

            // Cache old values before switching shader
            Color baseColor = mat.HasProperty("_Color") ? mat.GetColor("_Color") : Color.white;
            Texture mainTex = mat.HasProperty("_MainTex") ? mat.GetTexture("_MainTex") : null;
            Vector2 mainTexScale = mat.HasProperty("_MainTex") ? mat.GetTextureScale("_MainTex") : Vector2.one;
            Vector2 mainTexOffset = mat.HasProperty("_MainTex") ? mat.GetTextureOffset("_MainTex") : Vector2.zero;
            Texture bumpMap = mat.HasProperty("_BumpMap") ? mat.GetTexture("_BumpMap") : null;
            float bumpScale = mat.HasProperty("_BumpScale") ? mat.GetFloat("_BumpScale") : 1f;
            Texture emissionMap = mat.HasProperty("_EmissionMap") ? mat.GetTexture("_EmissionMap") : null;
            Color emissionColor = mat.HasProperty("_EmissionColor") ? mat.GetColor("_EmissionColor") : Color.black;
            float metallic = mat.HasProperty("_Metallic") ? mat.GetFloat("_Metallic") : 0f;
            float smoothness = mat.HasProperty("_Glossiness") ? mat.GetFloat("_Glossiness") : 0.5f;
            Texture metallicGlossMap = mat.HasProperty("_MetallicGlossMap") ? mat.GetTexture("_MetallicGlossMap") : null;
            Texture occlusionMap = mat.HasProperty("_OcclusionMap") ? mat.GetTexture("_OcclusionMap") : null;
            float occlusionStrength = mat.HasProperty("_OcclusionStrength") ? mat.GetFloat("_OcclusionStrength") : 1f;

            mat.shader = shader;

            mat.SetColor("_BaseColor", baseColor);
            if (mainTex != null)
            {
                mat.SetTexture("_BaseMap", mainTex);
                mat.SetTextureScale("_BaseMap", mainTexScale);
                mat.SetTextureOffset("_BaseMap", mainTexOffset);
            }
            if (bumpMap != null)
            {
                mat.SetTexture("_BumpMap", bumpMap);
                mat.SetFloat("_BumpScale", bumpScale);
                mat.EnableKeyword("_NORMALMAP");
            }
            if (emissionMap != null || emissionColor != Color.black)
            {
                mat.SetTexture("_EmissionMap", emissionMap);
                mat.SetColor("_EmissionColor", emissionColor);
                mat.EnableKeyword("_EMISSION");
                mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.BakedEmissive;
            }
            mat.SetFloat("_Metallic", metallic);
            mat.SetFloat("_Smoothness", smoothness);
            if (metallicGlossMap != null)
            {
                mat.SetTexture("_MetallicGlossMap", metallicGlossMap);
                mat.EnableKeyword("_METALLICSPECGLOSSMAP");
            }
            if (occlusionMap != null)
            {
                mat.SetTexture("_OcclusionMap", occlusionMap);
                mat.SetFloat("_OcclusionStrength", occlusionStrength);
            }

            EditorUtility.SetDirty(mat);
            upgraded++;
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Polyquest material upgrade complete: {upgraded} upgraded, {skipped} already URP.");
        EditorUtility.DisplayDialog(
            "Upgrade Complete",
            $"Upgraded {upgraded} materials to URP/Lit.\n{skipped} were already URP.",
            "OK"
        );
    }
}
