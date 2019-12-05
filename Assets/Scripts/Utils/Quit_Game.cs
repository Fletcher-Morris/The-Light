using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Quit_Game : MonoBehaviour
{
    public void QuitGame()
    {
#if UNITY_EDITOR
        if (Application.isEditor) UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
