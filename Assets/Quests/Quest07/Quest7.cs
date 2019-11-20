using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quest7 : Quest
{
    GameObject player;
    GameObject cam;
    GameObject watermillpoint;
    [SerializeField] GameObject uis;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
     
        watermillpoint = GameObject.FindGameObjectWithTag("Watermillpoint");
        QuestManager.instance.AddQuest(this);
        
    }
    public override void OnAdd()
    {
        Instantiate(uis);
        player.transform.position = watermillpoint.transform.position;
        Player_Controller.Singleton().CamYAxis().eulerAngles = new Vector3(0, 0, 0);
    }
    public override void Check()
    {
        
            QuestManager.instance.FinishQuest(this);
        
    }
}
