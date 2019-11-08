using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToWaterMill : MonoBehaviour
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
                if (QuestManager.instance.CheckQuestFinished("Escape") == true)
                {
                    triggered = true;
                    QuestManager.instance.GetQuest("Run").SetCondition("qc", 1);

                }
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
