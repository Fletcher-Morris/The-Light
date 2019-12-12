using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class TriggerDarkness : MonoBehaviour
{
    [SerializeField] private PlayableDirector director;
    [SerializeField] private List<GameObject> Darkclouds = new List<GameObject>();
    [SerializeField] private Camera m_cinematicCam, m_mainCam;
    [SerializeField] private GameObject Beaconlight;

    [SerializeField] private Environment_Transition m_environmentTransition;
    [SerializeField] private EnvironmentSettings m_darkEnvironment;

    public void Darkcome()
    {
        Player_Controller.Singleton().EnterCutscene();
        m_cinematicCam.enabled = true;
        m_mainCam.enabled = false;
        m_environmentTransition.Transition(m_darkEnvironment, 5.0f);
        director.Play();
        Player_Controller.Singleton().TeleportPlayer(new Vector3(98.5f, 62.5f, -441.5f), Vector3.right, new Vector2(0, 82.5f));
    }

    public void Turnoffbeacon()
    {
        Beaconlight.SetActive(false);
    }

    public void BackToGame()
    {
        m_cinematicCam.enabled = false;
        m_mainCam.enabled = true;
        Player_Controller.Singleton().ExitCutscene();
        
    }

    public void ActiveDarkclouds()
    {
        foreach(GameObject g in Darkclouds)
        {
            g.SetActive(true);
        }
    }
}
