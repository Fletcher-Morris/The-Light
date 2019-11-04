using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bluecube : MonoBehaviour
{
    bool destory = false;
    bool act = false;
    public GameObject ga;


    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")&&!act)
        {
            GameProcessManager.instance.ChangeCondition("bluecube", 1);
            ga.GetComponent<delemt>().c--;
            destory = true;
            act = true;
        }

    }
    private void LateUpdate()
    {

        if (destory)
        {
            Destroy(this.gameObject);
        }
    }
}