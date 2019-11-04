using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest2 : Quest
{
    public GameObject bluecube;
    public Quest next;

    private void Start()
    {
        GameProcessManager.instance.AddQuest(this);
    }

    public override void OnAdd()
    {
    Instantiate(bluecube);
    }

    public override void OnFinish()
    {
        GameProcessManager.instance.SetQuestActive(next);
    }

    public override void Check()
    {
        if (GameProcessManager.instance.ReadCondition("bluecube") >= 3)
        {
            GameProcessManager.instance.ChangeCondition("bluecube", -3);
            GameProcessManager.instance.ChangeCondition("bluecubequestfinish", 1);
            GameProcessManager.instance.FinishQuest(this);
        }
    }

}