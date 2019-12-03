using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class escapevillage : MonoBehaviour
{
    bool triggered = false;
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
                if (QuestManager.Instance.CheckQuestFinished("Fight Darkness") == true)
                {
                    triggered = true;
                    QuestManager.Instance.GetQuest("Escape").SetCondition("qc", 1);
                   
                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
