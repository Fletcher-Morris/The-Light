using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest1 : Quest
{
    Inventory_Controller ic;
    private void Start()
    {
        ic = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory_Controller>();
        QuestManager.instance.AddQuest(this);
    }
    public override void Check()
    {
        if (ic.HasItemInInventory("Parchment"))
        {
            QuestManager.instance.FinishQuest(this);
        }
    }
 
}
