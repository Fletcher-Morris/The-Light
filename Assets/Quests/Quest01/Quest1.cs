using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest1 : Quest
{
    private void Start()
    {
        QuestManager.Instance.AddQuest(this);
    }
    public override void Check()
    {
        if (Inventory_Controller.Singleton().HasItemInInventory("Parchment"))
        {
            QuestManager.Instance.FinishQuest(this);
        }
    }
 
}
