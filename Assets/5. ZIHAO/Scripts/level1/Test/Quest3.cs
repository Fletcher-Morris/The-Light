using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest3 : Quest
{
    public GameObject gate;
    void Start()
    {
        GameProcessManager.instance.AddQuest(this);
    }

    public override void OnQuestSetActive()
    {
        Destroy(gate);
        GameProcessManager.instance.SetCondition("lvdone", 1);
    }

}
