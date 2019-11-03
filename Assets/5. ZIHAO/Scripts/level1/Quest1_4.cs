using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest1_4 : Quest
{
    public GameObject coliders;
    public GameObject a;
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

    }
    public override void Check()
    {
        if(GameProcessManager.instance.ReadCondition("cloudclear") == 1)
        {
            GameProcessManager.instance.FinishQuest(this);
        }
        
    }
    public override void OnFinish()
    {
        GameProcessManager.instance.SetCondition("lvdone",1);
        Destroy(coliders);
        Destroy(a);
    }
}