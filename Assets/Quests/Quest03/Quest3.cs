using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest3 : Quest
{
    // Start is called before the first frame update
    void Start()
    {
        QuestManager.Instance.AddQuest(this); 
    }

    public override void Check()
    {
        if (this.ReadCondition("action") == 1)
        {
            QuestManager.Instance.FinishQuest(this);
        }
    }
    public override void OnFinish()
    {

        base.OnFinish();
    }
}
