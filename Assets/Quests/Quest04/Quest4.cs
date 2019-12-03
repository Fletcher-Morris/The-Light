using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest4 : Quest
{
    
    Inventory_Controller ic;
    // Start is called before the first frame update
    void Start()
    {
        QuestManager.Instance.AddQuest(this);
    }
 
    public override void OnFinish()
    {
        //GameObject.FindGameObjectWithTag("Player").GetComponent<Getlantern>().Showlantern();
        base.OnFinish();
    }

    public override void Check()
    {
       
        if(Inventory_Controller.Singleton().HasItemInInventory("The Lamp"))
        {
            QuestManager.Instance.FinishQuest(this);
        }
    }
}
