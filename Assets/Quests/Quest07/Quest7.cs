﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest7 : Quest
{
    GameObject cam;
    [SerializeField] GameObject uis;
    // Start is called before the first frame update
    void Start()
    {
        QuestManager.instance.AddQuest(this);        
    }
    public override void OnAdd()
    {
        Instantiate(uis);
        Player_Controller.Singleton().transform.position = new Vector3(249.65f, 25.31f, -191.28f);
        Player_Controller.Singleton().CamYAxis().eulerAngles = new Vector3(0, 0, 0);
    }
    public override void Check()
    {
        QuestManager.instance.FinishQuest(this);        
    }
}
