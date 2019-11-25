using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct LoadSceneFlags
{
    public string SceneName;
}

public static class SceneLoadingFlags
{
    private static LoadSceneFlags m_flags;
    public static LoadSceneFlags GetFlags() { return m_flags; }
    public static void SetFlags(LoadSceneFlags _flags) { m_flags = _flags; }
}