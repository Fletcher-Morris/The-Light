using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest6 : Quest
{
    // Start is called before the first frame update
    void Start()
    {
        QuestManager.Instance.AddQuest(this);
    }

    public override void Check()
    {
        if (this.ReadCondition("qc") == 1)
        {
            QuestManager.Instance.FinishQuest(this);
        }
    }
}
