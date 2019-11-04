using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest1_2 : Quest
{
    public Quest nextquest;
    // Start is called before the first frame update
    void Start()
    {
        GameProcessManager.instance.AddQuest(this);
    }
    public override void OnAdd()
    {

    }
    public override void OnQuestSetActive()
    {
        GameProcessManager.instance.SetCondition("doorenable", 1);
    }
    public override void Check()
    {
        if (GameProcessManager.instance.ReadCondition("dooropen")==1)
        {
            GameProcessManager.instance.FinishQuest(this);
        }
    }
    public override void OnFinish()
    {
        GameProcessManager.instance.SetQuestActive(nextquest);
    }
}
