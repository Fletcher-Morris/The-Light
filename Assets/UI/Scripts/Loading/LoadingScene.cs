using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] Text m_Text; //TO DO:Get the text

    // Start is called before the first frame update
    void Start()
    {
        int SceneToLoad = 2; //2 is the ID of the Game scene
        if (PlayerPrefs.HasKey("SceneToLoad"))
            SceneToLoad = PlayerPrefs.GetInt("SceneToLoad");

        CallLoadScene(SceneToLoad);
    }

    public void CallLoadScene(int SceneID)
    {
        StartCoroutine(LoadScene(SceneID));
    }

    IEnumerator LoadScene(int SceneID)
    {
        //Begin to load the Scene you specify
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(SceneID);
        //Don't let the Scene activate until you allow it to
        asyncOperation.allowSceneActivation = false;
        //When the load is still in progress, output the Text and progress bar
        while (!asyncOperation.isDone)
        {
            //Output the current progress
            m_Text.text = (asyncOperation.progress * 100) + "%";

            // Check if the load has finished
            if (asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;
            }

            /*yield return null;*/
            yield return new WaitForSeconds(1f);
        }
        yield return null;
    }
}
