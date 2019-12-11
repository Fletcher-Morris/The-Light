using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

public class Override_Lamps : Editor
{

    static int m_override = 0;

    [MenuItem("Tools/Toggle Lamp Override")]
    public static void ConvertMaterials()
    {
        m_override = 1 - m_override;
        Shader.SetGlobalInt("OverrideLamps", m_override);
    }

    static GameObject management;
    [MenuItem("Tools/Reset Environment")]
    public static void ResetEnvironment()
    {
        if(management == null) management = GameObject.Find("~~~MANAGEMENT~~~");
        if(management == null) management = GameObject.Find("~~~MANAGEMENT~~~ ");
        if(management == null) { Debug.LogWarning("Could not locate management object!"); return; }
        if(management.TryGetComponent<Environment_Transition>(out Environment_Transition env))
        {
            env.LerpToEnvironment(env.GetCurrentEnvironment(), 1.0f, false);
        }
    }
}

#endif