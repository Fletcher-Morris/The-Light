using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectLantern : MonoBehaviour
{
    IEnumerator Coroutine;
    [SerializeField] Transform LanternPlace;
    [SerializeField] GameObject lantern;
    [SerializeField] Animator Fireflies;

    private void Awake()
    {
        lantern.SetActive(false);
    }

    public void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("Player"))
        {
            //Check if player presses E key
            Coroutine = CheckCollection();
            StartCoroutine(Coroutine);
        }
    }

    public void OnTriggerExit(Collider c)
    {
        if (c.CompareTag("Player") && Coroutine != null)
            StopCoroutine(Coroutine);
        //Stop checking
    }

    IEnumerator CheckCollection()
    {
        //Check when Escape is pressed to delete the parchment
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                lantern.SetActive(true);

                transform.parent = LanternPlace;
                transform.localPosition = new Vector3(0f, 0f, 0f);
                transform.localRotation = new Quaternion(0f, 0f, 0f, 0f);

                Fireflies.Play("Fireflies2");
            }
            yield return null;
        }
    }
}
