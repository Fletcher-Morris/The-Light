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
         ic = Inventory_Controller.Singleton();
    }
 
    public override void OnFinish()
    {
        //GameObject.FindGameObjectWithTag("Player").GetComponent<Getlantern>().Showlantern();
        base.OnFinish();
    }

    public override void Check()
    {
       
        if(ic.HasItemInInventory("The_Lamp"))
        {
            QuestManager.instance.FinishQuest(this);
        }
    }
}
