using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDarkness : MonoBehaviour
{
    bool triggered=false;
    public List<GameObject> gameObjects = new List<GameObject>();
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
        foreach(GameObject ga in gameObjects)
        {
            ga.SetActive(true);
        }
        gameObjects[0].GetComponent<ParticleSystem>().Play();
        gameObjects[1].GetComponent<ParticleSystem>().Play();
    }
}
