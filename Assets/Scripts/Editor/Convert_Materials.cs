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



    [MenuItem("Tools/Fix Fence Colliders")]
    public void FixFenceColliders()
    {
        List<Vector3> fenceMainWorldPositions = new List<Vector3>();
        List<Quaternion> fenceMainWorldRotations = new List<Quaternion>();
        List<Transform> fenceMainParents = new List<Transform>();
        List<Vector3> fenceEndWorldPositions = new List<Vector3>();
        List<Quaternion> fenceEndWorldRotations = new List<Quaternion>();
        List<Transform> fenceEndParents = new List<Transform>();

        GameObject fenceMainPrefab = GameObject.Find("Fence_Main_Prefab");
        GameObject fenceEndPrefab = GameObject.Find("Fence_End_Prefab");

        if (fenceMainPrefab == null) return;
        if (fenceEndPrefab == null) return;

        List<GameObject> allFoundFences = new List<GameObject>(GameObject.FindGameObjectsWithTag("FixFence"));

        if (allFoundFences.Count <= 0) return;

        foreach(GameObject g in allFoundFences)
        {
            if(g.name.Contains("fence00"))
            {
                fenceMainWorldPositions.Add(g.transform.position);
                fenceMainWorldRotations.Add(g.transform.rotation);
                fenceMainParents.Add(g.transform.parent);
                GameObject.Destroy(g);
            }
            else if(g.name.Contains("fence_end"))
            {
                fenceEndWorldPositions.Add(g.transform.position);
                fenceEndWorldRotations.Add(g.transform.rotation);
                fenceEndParents.Add(g.transform.parent);
                GameObject.Destroy(g);
            }
        }

        for(int i = 0; i < fenceMainWorldPositions.Count; i++)
        {
            GameObject newFence = Instantiate(fenceMainPrefab, fenceMainWorldPositions[i], fenceMainWorldRotations[i], fenceMainParents[i]);
        }
        for (int i = 0; i < fenceEndWorldPositions.Count; i++)
        {
            GameObject newFence = Instantiate(fenceEndPrefab, fenceEndWorldPositions[i], fenceEndWorldRotations[i], fenceEndParents[i]);
        }

        Debug.Log($"Replaced {(fenceMainWorldPositions.Count + fenceEndWorldPositions.Count).ToString()} fences with prefabs!");
    }
}

#endif