using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    [SerializeField] private GameObject bird;
    [SerializeField] private Camera m_cinematicCam, m_mainCam;

    public void PlayAnimation()
    {
        m_mainCam.enabled = false;
        m_cinematicCam.enabled = true;
        Player_Controller.Singleton().InCutscene = true;
    }

    public void Backtogame()
    {
        bird.SetActive(false);
        m_cinematicCam.enabled = false;
        m_mainCam.enabled = true;
        Player_Controller.Singleton().InCutscene = false;
    }
}
