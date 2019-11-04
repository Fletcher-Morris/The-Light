using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateCorruption : MonoBehaviour
{
    [SerializeField] Corruption corruption;

    public void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("Player"))
        {
            corruption.PlayCorruption();
        }
    }
}
