using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest1 : Quest
{
    Inventory_Controller ic;
    private void Start()
    {
        ic = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory_Controller>();
        QuestManager.Instance.AddQuest(this);
    }
    public override void Check()
    {
        if (ic.HasItemInInventory("Parchment"))
        {
            QuestManager.Instance.FinishQuest(this);
        }
    }
 
}
