using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ai_Waypoint : MonoBehaviour
{
    [SerializeField] private List<Ai_Settings> m_excludedAi;
    public List<Ai_Settings> GetExcludedAi() { return m_excludedAi; }
    private void Awake()
    {
        Ai_Manager.AddWaypoint(this);
    }
}
