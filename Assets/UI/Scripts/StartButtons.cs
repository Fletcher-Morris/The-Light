using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButtons : MonoBehaviour
{

    [SerializeField] GameObject Credits;


    public void OnClickStart()
    {//Go to the loading Scene

        PlayerPrefs.SetInt("SceneToLoad", 2); //2 is the ID of the game Scene
        SceneManager.LoadScene(1); // 1 is the ID of the loading scene
    }

    public void OnClickCredits()
    {
        //Activate the Canvas for the credits -> Can change to a separate scene
        Credits.SetActive(true);
    }

    public void OnClickQuit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit ();
#endif
    }
}
