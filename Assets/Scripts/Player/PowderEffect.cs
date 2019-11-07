using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PowderEffect
{
    [SerializeField] private float m_remainingTime;
    public float RemainingTime { get => m_remainingTime; set => m_remainingTime = value; }
    public Powder Powder { get => m_powder; set => m_powder = value; }
    [SerializeField] private Powder m_powder;


    public PowderEffect (Powder _powder)
    {
        m_powder = _powder;
    }
}
