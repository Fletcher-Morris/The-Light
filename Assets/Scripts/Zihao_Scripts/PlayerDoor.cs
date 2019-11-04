using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDoor : MonoBehaviour
{
    IEnumerator Coroutine;
    bool CanOpenDoor = false;
    [SerializeField] GameObject Colliders;
    public void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("Player"))
        {
            //Check if player presses E key
            Coroutine = CheckKey();
            StartCoroutine(Coroutine);  
        }
    }

    public void OnTriggerExit(Collider c)
    {
        if (Coroutine != null)
            StopCoroutine(Coroutine);
         //Stop checking
    }

    IEnumerator CheckKey()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (CanOpenDoor) //If step 1, check if player presses E to read parchment
                    StartCoroutine(OpenTheDoor());
                else //If step 2, check if player presses E to open the door
                    OpenTheMail(); 
            }
            yield return null;
        }
    }

    public void OpenTheMail()
    {
        GetComponentInChildren<Parchment>().MoveToCamera();
        CanOpenDoor = true;
    }

    IEnumerator OpenTheDoor()
    {
        AudioManager.instance.StartHelp();
        Destroy(Colliders);
        while (transform.rotation.y > 0.4f)
        {
            transform.Rotate(0, -0.75f, 0);
            yield return null;
        }

        //FindObjectOfType<Corruption>().PlayCorruption();
    }
}
