using UnityEngine;
using UnityEditor;

public class ConvertURPToStandard : EditorWindow
{
    [MenuItem("Tools/Convert URP Materials to Standard")]
    public static void Convert()
    {
        string[] materialGUIDs = AssetDatabase.FindAssets("t:Material");
        int count = 0;

        foreach (string guid in materialGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (mat == null || mat.shader == null) continue;

            // URP Lit → Standard
            if (mat.shader.name == "Universal Render Pipeline/Lit")
            {
                mat.shader = Shader.Find("Standard");

                // Mapping des propriétés similaires
                if (mat.HasProperty("_BaseColor"))
                    mat.SetColor("_Color", mat.GetColor("_BaseColor"));

                if (mat.HasProperty("_BaseMap"))
                    mat.SetTexture("_MainTex", mat.GetTexture("_BaseMap"));

                count++;
            }
        }

        Debug.Log("Conversion terminée, matériaux convertis : " + count);
    }
}
