using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest5 : Quest
{
    // Start is called before the first frame update
    void Start()
    {
        QuestManager.instance.AddQuest(this);
    }

    public override void Check()
    {
        this.ChangeCondition("timecount", 1);
        if (this.ReadCondition("timecount") >= 1000 || this.ReadCondition("killcount") >= 10)
        {
            QuestManager.instance.FinishQuest(this);
        }
    }
}
