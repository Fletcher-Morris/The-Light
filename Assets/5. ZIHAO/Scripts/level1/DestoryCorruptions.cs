using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryCorruptions : Quest
{
   public GameObject spwaner;
    GameObject a;
    void Start()
    {
        questname = "DestoryCorruption";
        describe = "Destory 8 corruption clouds";
        quest_active = true;
        GameProcessManager.instance.AddQuest(this);
    }

    public override void OnAdd()
    {
        a=Instantiate(spwaner);
    }

    public override void OnFinish()
    {
        Destroy(a);
        GameProcessManager.instance.SetCondition("cloudclear", 1);
    }
    public override void Check()
    {
        if (GameProcessManager.instance.ReadCondition("cloudkillcount") >= 8)
        {
            GameProcessManager.instance.FinishQuest(this);
        }
    }
}
