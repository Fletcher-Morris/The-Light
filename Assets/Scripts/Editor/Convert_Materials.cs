using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

public class Convert_Materials : Editor
{

    static Shader normalShader;
    static Shader autodeskShader;
    static Shader newShader;

    [MenuItem("Tools/Convert Materials")]
    public static void ConvertMaterials()
    {
        normalShader = Shader.Find("Lightweight Render Pipeline/Lit");
        autodeskShader = Shader.Find("Lightweight Render Pipeline/Autodesk Interactive/Autodesk Interactive");
        newShader = Shader.Find("Zelda_Shader");

        if (normalShader == null) Debug.LogWarning("Could not locate regular shader.");
        if (autodeskShader == null) Debug.LogWarning("Could not locate autodesk shader.");
        if (newShader == null) Debug.LogWarning("Could not locate zelda shader.");

        if (normalShader == null) return;
        if (autodeskShader == null) return;
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
        if (mat.shader == normalShader)
        {
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
        else if (mat.shader == autodeskShader)
        {
            Texture albedoTex;
            Texture emissionTex;
            Texture normalTex;
            albedoTex = mat.GetTexture("_MainTex");
            emissionTex = mat.GetTexture("_EmissionMap");
            normalTex = mat.GetTexture("_BumpMap");
            mat.shader = newShader;
            mat.SetTexture("_Albedo", albedoTex);
            mat.SetTexture("_Emission", emissionTex);
            mat.SetTexture("_Normal", normalTex);
            mat.SetFloat("_Terrain", 0.0f);
            return true;
        }

        return false;
    }



    [MenuItem("Tools/Fix Fences")]
    public static void FixFenceColliders()
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

        List<GameObject> allFoundFences = new List<GameObject>(GameObject.FindGameObjectsWithTag("Fence"));

        if (allFoundFences.Count <= 0) return;

        foreach (GameObject g in allFoundFences)
        {
            if (g != fenceMainPrefab && g != fenceEndPrefab)
            {
                if (g.name.Contains("fence00"))
                {
                    fenceMainWorldPositions.Add(g.transform.position);
                    fenceMainWorldRotations.Add(g.transform.rotation);
                    fenceMainParents.Add(g.transform.parent);
                    GameObject.DestroyImmediate(g);
                }
                else if (g.name.Contains("fence_end"))
                {
                    fenceEndWorldPositions.Add(g.transform.position);
                    fenceEndWorldRotations.Add(g.transform.rotation);
                    fenceEndParents.Add(g.transform.parent);
                    GameObject.DestroyImmediate(g);
                }
                else if (g.name.Contains("Fence_Main_Prefab"))
                {
                    fenceMainWorldPositions.Add(g.transform.position);
                    fenceMainWorldRotations.Add(g.transform.rotation);
                    fenceMainParents.Add(g.transform.parent);
                    GameObject.DestroyImmediate(g);
                }
                else if (g.name.Contains("Fence_End_Prefab"))
                {
                    fenceEndWorldPositions.Add(g.transform.position);
                    fenceEndWorldRotations.Add(g.transform.rotation);
                    fenceEndParents.Add(g.transform.parent);
                    GameObject.DestroyImmediate(g);
                }
            }
        }

        for (int i = 0; i < fenceMainWorldPositions.Count; i++)
        {
            GameObject newFence = Instantiate(fenceMainPrefab, fenceMainWorldPositions[i], fenceMainWorldRotations[i], fenceMainParents[i]);
        }
        for (int i = 0; i < fenceEndWorldPositions.Count; i++)
        {
            GameObject newFence = Instantiate(fenceEndPrefab, fenceEndWorldPositions[i], fenceEndWorldRotations[i], fenceEndParents[i]);
        }

        Debug.Log($"Replaced {(fenceMainWorldPositions.Count + fenceEndWorldPositions.Count).ToString()} fences with prefabs!");
    }

    [MenuItem("Tools/Fix Grass")]
    public static void FixGrass()
    {
        foreach (Terain_Grass_Extract grass in FindObjectsOfType<Terain_Grass_Extract>())
        {
            grass.refresh = true;
        }
    }
}

#endif