using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class redcube : MonoBehaviour
{
    bool destory=false;

    bool act = false;
  

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")&&!act)
        {
            act = true;
            GameProcessManager.instance.ChangeCondition("redcube", 1);
            destory = true;
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
