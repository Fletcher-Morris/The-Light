using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Async_Scene_Loader : MonoBehaviour
{

    [SerializeField] private Text m_progressText;

    void Start()
    {
        if (m_progressText == null) m_progressText = GameObject.Find("LOADING_PROGRESS_TEXT").GetComponent<Text>();
        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        yield return null;

        LoadSceneFlags flags = SceneLoadingFlags.GetFlags();
        AsyncOperation asynchOp = SceneManager.LoadSceneAsync(flags.SceneName);
        asynchOp.allowSceneActivation = false;

        while(asynchOp.isDone == false)
        {
            m_progressText.text = (asynchOp.progress * 100) + "%";

            if(asynchOp.progress >= 0.9f)
            {
                asynchOp.allowSceneActivation = true;
                OpenScene();
            }
            yield return null;
        }

    }

    private void OpenScene()
    {

    }
}
