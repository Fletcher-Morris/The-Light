using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectparchment : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void Collect()
    {
        QuestManager.instance.GetQuest("Collect Parchment").SetCondition("parchmentcollected", 1);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
