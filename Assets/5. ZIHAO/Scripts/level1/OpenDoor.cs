using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    [SerializeField] GameObject Colliders;
    bool opened = false;
    IEnumerator DoorOp()
    {
        Destroy(Colliders);
        for(int i=0;i<150;i++)
        {
            transform.Rotate(0, -0.75f, 0);
            yield return null;
        }

    }
    public void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameProcessManager.instance.ReadCondition("doorenable") ==1&&!opened)
            {
                opened = true;
                StartCoroutine(DoorOp());
                GameProcessManager.instance.SetCondition("dooropen", 1);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
