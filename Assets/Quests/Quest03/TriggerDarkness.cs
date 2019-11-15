using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

public class TriggerDarkness : MonoBehaviour
{
    bool triggered=false;
    public PlayableDirector director;
    public List<GameObject> Darkclouds = new List<GameObject>();
    public GameObject CM, CC,PL;
    public GameObject Beaconlight;
  
    // Start is called before the first frame update
    void Start()
    {
     
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!triggered)
        {
            
            if (other.CompareTag("Player"))
            {
                if (QuestManager.instance.CheckQuestFinished("Read Parchment") == true)
               {
                    triggered = true;
                    
                   QuestManager.instance.GetQuest("Go To The Tower").SetCondition("action", 1);
                    Darkcome();
               }
            }
        }
    }
    void Darkcome()
    {
        CM.SetActive(false);
        PL.SetActive(false);
        CC.SetActive(true);
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
        PL.SetActive(true);
        QuestManager.instance.SetQuestActive("Get the Lantern");
    }
    public void ActiveDarkclouds()
    {
        foreach(GameObject g in Darkclouds)
        {
            g.SetActive(true);
        }
    }
}
