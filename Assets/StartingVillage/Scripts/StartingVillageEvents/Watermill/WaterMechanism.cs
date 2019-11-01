using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMechanism : MonoBehaviour
{
    IEnumerator Coroutine;

    public bool hasbeenActivated;
    [SerializeField] WaterMechanism dependencies;
    [SerializeField] Animator Anim;

    private void Awake()
    {
        hasbeenActivated = false;
        Anim.enabled = false;
    }

    public void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("Player"))
        {
            //Check if player presses E key
            Coroutine = CheckMechanism();
            StartCoroutine(Coroutine);
        }
    }

    public void OnTriggerExit(Collider c)
    {
        if (Coroutine != null)
            StopCoroutine(Coroutine);
        //Stop checking
    }

    IEnumerator CheckMechanism()
    {
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.E) && (dependencies == null || dependencies.hasbeenActivated))
            {
                hasbeenActivated = true;
                Anim.enabled = true;
            }
            yield return null;
        }
    }
}
