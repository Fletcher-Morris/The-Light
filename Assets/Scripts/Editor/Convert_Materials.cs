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
        List<Material> convertedMats = new List<Material>();
        foreach (Material mat in foundMaterials)
        {
            if (ConverteMaterial(mat)) convertedMats.Add(mat);
        }

        Debug.Log("Converted " + convertedMats.Count + " materials to game shader.");
    }

    private static bool ConverteMaterial(Material mat)
    {
        if (mat.shader != normalShader) return false;


        Texture albedoTex;
        Texture emissionTex;
        Texture normalTex;

        albedoTex = mat.GetTexture("_BaseMap");
        emissionTex = mat.GetTexture("_EmissionMap");
        normalTex = mat.GetTexture("_BumpMap");

        mat.shader = newShader;

        mat.SetTexture("_Albedo", albedoTex);
        mat.SetTexture("_Emission", emissionTex);
        mat.SetTexture("_Normal", normalTex);
        mat.SetFloat("_Terrain", 0.0f);

        return true;
    }
}

#endif