﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class TriggerDarkness : MonoBehaviour
{
    bool triggered=false;
    public PlayableDirector director;
    public List<GameObject> Darkclouds = new List<GameObject>();
    public GameObject CM, CC;
    public GameObject Beaconlight;
    public GameObject quest;

    [SerializeField] private Environment_Transition m_environmentTransition;
    [SerializeField] private EnvironmentSettings m_darkEnvironment;

    private void OnTriggerEnter(Collider other)
    {
        if (!triggered)
        {
            
            if (other.CompareTag("Player"))
            {
               // if (QuestManager.instance.CheckQuestFinished("Read Parchment") == true)
              // {
                    triggered = true;
                   QuestManager.Instance.ClearQuestList();
                  // QuestManager.instance.GetQuest("Go To The Tower").SetCondition("action", 1);
                    Darkcome();
               //}
            }
        }
    }
    void Darkcome()
    {
        Player_Controller.Singleton().InCutscene = true;
        CM.SetActive(false);
        CC.SetActive(true);
        m_environmentTransition.Transition(m_darkEnvironment, 5.0f);
        director.Play();
    }
    public void Turnoffbeacon()
    {
        Beaconlight.SetActive(false);
    }
   public void BackToGame()
    {
        CC.SetActive(false);
        CM.SetActive(true);
        Player_Controller.Singleton().InCutscene = false;
        //QuestManager.instance.SetQuestActive("Get the Lantern");
        Instantiate(quest);
    }
    public void ActiveDarkclouds()
    {
        foreach(GameObject g in Darkclouds)
        {
            g.SetActive(true);
        }
    }
}
