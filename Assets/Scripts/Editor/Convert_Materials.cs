using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

public class Convert_Materials : Editor
{

    static Shader normalShader;
    static Shader newShader;

    [MenuItem("Tools/Convert Materials")]
    public static void ConvertMaterials()
    {
        normalShader = Shader.Find("Lightweight Render Pipeline/Lit");
        newShader = Shader.Find("Zelda_Shader");

        if (normalShader == null) Debug.LogWarning("Could not locate regular shader.");
        if (newShader == null) Debug.LogWarning("Could not locate zelda shader.");

        if (normalShader == null) return;
        if (newShader == null) return;

        List<Material> foundMaterials = new List<Material>();
        foundMaterials.AddRange(Resources.FindObjectsOfTypeAll<Material>());
        Debug.Log("Converting " + foundMaterials.Count + " Materials");
        foreach (Material mat in foundMaterials)
        {
            ConverteMaterial(mat);
        }
    }

    private static void ConverteMaterial(Material mat)
    {
        if (mat.shader != normalShader) return;


        Texture albedoTex;
        Texture emissionTex;
        Texture normalTex;

        albedoTex = mat.GetTexture("_BaseMap");
        emissionTex = mat.GetTexture("_EmissionMap");
        normalTex = mat.GetTexture("_BumpMap");

        Debug.Log("Converting Material : " + mat + ", " + albedoTex);

        mat.shader = newShader;

        mat.SetTexture("_Albedo", albedoTex);
        mat.SetTexture("_Emission", emissionTex);
        mat.SetTexture("_Normal", normalTex);
        mat.SetFloat("_Terrain", 0.0f);
    }
}

#endif