using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Activate_Quest : MonoBehaviour
{
    [SerializeField] private GameObject m_quest;

    public void ActivateQuest()
    {
        if(enabled) GameObject.Instantiate(m_quest);
    }
}
