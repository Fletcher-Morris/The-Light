using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Openmenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            QuestManager.instance.GetQuest("Read Parchment").SetCondition("openmenu", 1);
        }  
    }
}
