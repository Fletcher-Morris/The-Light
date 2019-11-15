using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanCamp_CorruptionEffect : MonoBehaviour
{
    private void Awake()
    {
        GetComponent<Animator>().enabled = false;
    }


    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Corruption"))
        {
            GetComponent<Animator>().enabled = true;
            GetComponent<BoxCollider>().enabled = false; //Make sure the animation doesn't start again
        }
    }
}
