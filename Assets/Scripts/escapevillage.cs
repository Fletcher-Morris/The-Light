using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class escapevillage : MonoBehaviour
{
    bool triggered = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!triggered)
        {

            if (other.CompareTag("Player"))
            {
               
                   triggered = true;
                   Gotowatermill();
            }
        }
    }

    void Gotowatermill()
    {
                //Player_Controller.Singleton().transform.position = new Vector3(249.65f, 25.31f, -191.28f);
               // Player_Controller.Singleton().CamYAxis().eulerAngles = new Vector3(0, 0, 0);
    }

}
