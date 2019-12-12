using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Load_Scene))]
[RequireComponent(typeof(Player_Trigger))]
public class OnEnd : MonoBehaviour
{

    public void EndGame()
    {
        if (m_started) return;
        m_started = true;
        StartCoroutine(EndGameCoroutine());
    }

    private void Start()
    {
        m_sceneLoader = GetComponent<Load_Scene>();
        m_blackFadeoutImage = Player_Controller.Singleton().transform.GetChild(0).Find("TBC").GetComponent<CanvasGroup>();
        m_tbcText = m_blackFadeoutImage.transform.GetChild(0).GetComponent<Text>();
    }

    private CanvasGroup m_blackFadeoutImage;
    private Text m_tbcText;
    private Load_Scene m_sceneLoader;
    [SerializeField] private Transform m_runToPos;

    private bool m_started = false;

    private IEnumerator EndGameCoroutine()
    {
        yield return null;

        m_blackFadeoutImage.alpha = 0.0f;
        Color col = new Color(1, 1, 1, 0);
        m_tbcText.color = col;
        float t = 0.0f;

        Player_Controller.Singleton().EnterCutscene(false);
        Player_Controller.Singleton().MoveToPos = m_runToPos;

        while(t < 4.0f)
        {
            t += GameTime.deltaTime;
            float c = t / 4.0f;
            c = c.Clamp01();
            m_blackFadeoutImage.alpha = c;
            yield return new WaitForEndOfFrame();
        }
        t = 0.0f;
        while (t < 2.0f)
        {
            t += GameTime.deltaTime;
            float c = t / 2.0f;
            c = c.Clamp01();
            col.a = c;
            m_tbcText.color = col;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSecondsRealtime(3.0f);
        t = 0.0f;
        while (t < 2.0f)
        {
            t += GameTime.deltaTime;
            float c = t / 2.0f;
            c = c.Clamp01();
            col.a = 1.0f - c;
            m_tbcText.color = col;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSecondsRealtime(0.25f);
        m_sceneLoader.LoadScene();
    }
}
