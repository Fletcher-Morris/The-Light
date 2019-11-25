using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Load_Scene : MonoBehaviour
{
    [SerializeField] private string m_sceneToLoad;
    [SerializeField] private bool m_useLoadingScreen = true;
    [SerializeField] private bool m_useLoadingFlags = false;
    [SerializeField] private LoadSceneFlags m_flags;

    public void LoadScene(string _sceneName)
    {
        if(SceneManager.GetSceneByName(_sceneName) == null)
        {
            Debug.LogWarning("Scene '" + _sceneName + "' does not exist in the build!");
            return;
        }
        else
        {
            if(!m_useLoadingFlags)
            {
                m_flags = SceneLoadingFlags.GetFlags();
                m_flags.SceneName = _sceneName;
            }
            SceneLoadingFlags.SetFlags(m_flags);
            Debug.Log("Loading Scene '" + SceneLoadingFlags.GetFlags().SceneName + "'.");
            if(m_useLoadingScreen)
            {
                SceneManager.LoadScene("Loading_Screen_Scene");
            }
            else
            {
                SceneManager.LoadScene(SceneLoadingFlags.GetFlags().SceneName);
            }

        }
    }

    public void LoadScene()
    {
        LoadScene(m_sceneToLoad);
    }
}
