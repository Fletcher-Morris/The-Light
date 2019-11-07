﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest2 : Quest
{
    // Start is called before the first frame update
    void Start()
    {
        QuestManager.instance.AddQuest(this);
    }

    public override void Check()
    {
        if (this.ReadCondition("openmenu") == 1)
        {
            QuestManager.instance.FinishQuest(this);
        }
    }
}
