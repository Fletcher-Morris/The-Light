using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest9 : Quest
{
    // Start is called before the first frame update
    void Start()
    {
        QuestManager.Instance.AddQuest(this);
    }

    
}
