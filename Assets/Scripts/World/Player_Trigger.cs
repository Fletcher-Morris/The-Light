using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player_Trigger : MonoBehaviour
{
    [SerializeField] private bool m_singleUse = true;
    private bool m_triggered = false;
    [SerializeField] private bool m_deathTrigger = false;

    [SerializeField] private UnityEvent OnTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject == Player_Controller.Singleton().gameObject)
        {
            Debug.Log("Triggered [" + gameObject + "]");
            if (m_deathTrigger) Player_Controller.Singleton().KillPlayer();
            OnTrigger.Invoke();
        }
    }
}
