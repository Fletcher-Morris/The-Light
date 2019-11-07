using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ai_Waypoint : MonoBehaviour
{
    [SerializeField] private List<Ai_Settings> m_includedAi;
    public List<Ai_Settings> GetIncludedAi() { return m_includedAi; }

    [SerializeField] private bool m_autoPosition = true;


    private void Awake()
    {
        if(m_autoPosition) AutoPosition();
        Ai_Manager.AddWaypoint(this);
    }

    private void AutoPosition()
    {
        RaycastHit hit;
        Ray ray = new Ray(new Vector3(transform.position.x, 1000, transform.position.z), Vector3.down);
        Physics.Raycast(ray, out hit, 2000.0f, LayerTools.CreateLayerMask("Terrain").AddLayer("Ground"));
        if(hit.collider)
        {
            transform.position = hit.point + new Vector3(0, 0.5f, 0);
        }
    }
}
