using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player_Trigger : MonoBehaviour
{
    [SerializeField] private bool m_singleUse = true;
    private bool m_triggered = false;
    [SerializeField] private float m_damageValue = 0.0f;

    [SerializeField] private bool m_triggerOnEnter = true;
    [SerializeField] private bool m_triggerOnStay = false;

    [SerializeField] private UnityEvent OnTrigger;

    private void OnTriggerEnter(Collider other)
    {
        if (m_triggerOnEnter == false) return;
        if(other.gameObject == Player_Controller.Singleton().gameObject)
        {
            Debug.Log("Triggered [" + gameObject + "]");
            if (m_damageValue != 0.0f) Player_Controller.Singleton().Health?.DoDamage(m_damageValue);
            OnTrigger.Invoke();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (m_triggerOnStay == false) return;
        if (other.gameObject == Player_Controller.Singleton().gameObject)
        {
            Debug.Log("Triggered [" + gameObject + "]");
            if (m_damageValue != 0.0f) Player_Controller.Singleton().Health?.DoDamage(m_damageValue);
            OnTrigger.Invoke();
        }
    }
}
