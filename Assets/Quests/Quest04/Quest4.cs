using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest4 : Quest
{
    
    Inventory_Controller ic;
    // Start is called before the first frame update
    void Start()
    {
        QuestManager.instance.AddQuest(this);
        ic = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory_Controller>();
    }
    public override void OnFinish()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<Getlantern>().Showlantern();
        base.OnFinish();
    }

    public override void Check()
    {
        if(ic.HasItemInInventory("The Lamp"))
        {
            QuestManager.instance.FinishQuest(this);
        }
    }
}
