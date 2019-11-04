using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest1 : Quest
{
    public GameObject redcube;
    public Quest next;
    public GameObject light;

    private void Start()
    {    
        GameProcessManager.instance.AddQuest(this);
    }

    public override void OnQuestSetActive()
    {
        Instantiate(redcube);
    }

    public override void OnFinish()
    {
        GameProcessManager.instance.SetQuestActive(next);
        RenderSettings.ambientIntensity = 0.1f;
        light.GetComponent<Light>().intensity = 0.2f;
    }

    public override void Check()
    {
        if (GameProcessManager.instance.ReadCondition("redcube") >= 1)
        {
            GameProcessManager.instance.ChangeCondition("redcube", -1);
            GameProcessManager.instance.ChangeCondition("redcubequestfinish", 1);
            GameProcessManager.instance.FinishQuest(this);
        }
    }

}