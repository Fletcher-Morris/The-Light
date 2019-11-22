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
}

#endif