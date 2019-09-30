using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Config_Bones : Editor
{
    [MenuItem("Tools/Configure Bones")]
    public static void ConfigureBones()
    {
        Transform root;
        root = Selection.activeGameObject.transform.parent;
        List<Transform> childBones = new List<Transform>();
        GameObject bonesRoot = Instantiate(new GameObject("Bones"), root);
        bonesRoot.name = "Bones";
        childBones.AddRange(Selection.activeGameObject.GetComponentsInChildren<Transform>());
        Debug.Log("Found " + childBones.Count + " Bones");
        foreach (Transform t in childBones)
        {
            GameObject boner = Instantiate(new GameObject("Bone"), t.position, t.rotation, bonesRoot.transform);
            t.SetParent(boner.transform);
            boner.name = "Bone";
        }
    }
}
