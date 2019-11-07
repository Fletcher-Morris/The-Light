using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest01 : Quest
{
    
    private void Start()
    {
        QuestManager.instance.AddQuest(this);
    }
    public override void Check()
    {
        if (this.ReadCondition("parchmentcollected") == 1)
        {
            QuestManager.instance.FinishQuest(this);
        }
    }
    public override void OnFinish()
    {
       
    }
}
