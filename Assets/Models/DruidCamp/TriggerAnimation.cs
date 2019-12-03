using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class TriggerAnimation : MonoBehaviour
{
    public PlayableDirector Animation;
    public GameObject ToHide;

    private void Awake()
    {
        Animation.enabled = false;
        if (ToHide != null)
            ToHide.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (ToHide != null)
                ToHide.SetActive(true);
            Animation.enabled = true;
            
        }
    }
}
