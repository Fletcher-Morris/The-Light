using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ai_Waypoint : MonoBehaviour
{
    [SerializeField] private List<Ai_Settings> m_includedAi;
    public List<Ai_Settings> GetIncludedAi() { return m_includedAi; }
    private void Awake()
    {
        Ai_Manager.AddWaypoint(this);
    }
}
