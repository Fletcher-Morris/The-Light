using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

public class Convert_Materials : Editor
{
    [MenuItem("Tools/Convert Materials")]
    public static void ConvertMaterials()
    {
        Shader normalShader = Shader.Find("Lightweight Render Pipeline/Lit");
        Shader newShader = Shader.Find("Zelda_Shader");

        if (normalShader == null) Debug.LogWarning("Could not locate regular shader.");
        if (newShader == null) Debug.LogWarning("Could not locate zelda shader.");

        if (normalShader == null) return;
        if (newShader == null) return;

        List<Material> foundMaterials = new List<Material>(Resources.FindObjectsOfTypeAll<Material>());
        foreach(Material mat in foundMaterials)
        {
            if(mat.shader == normalShader)
            {
                Texture albedoTex;
                Texture emissionTex;
                Texture normalTex;

                albedoTex = mat.GetTexture(0);
                emissionTex = mat.GetTexture(5);
                normalTex = mat.GetTexture(3);

                mat.shader = newShader;

                mat.SetTexture(0, albedoTex);
                mat.SetTexture(1, emissionTex);
                mat.SetTexture(2, normalTex);

            }
        }
    }
}

#endif