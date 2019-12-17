using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ai_Manager : MonoBehaviour
{
    private static Ai_Manager m_singleton;
    public static Ai_Manager Singleton() { return m_singleton; }


    const int MAX_LAMPS = 16;

    private int m_lastAiId = -1;
    public int GetNewAiId()
    {
        m_lastAiId++;
        return m_lastAiId;
    }

    [SerializeField] private List<Enemy_Ai> m_enemyAiList = new List<Enemy_Ai>();
    public int AddAi(Enemy_Ai _ai)
    {
        m_enemyAiList.Add(_ai);
        return GetNewAiId();
    }

    public List<Enemy_Ai> GetEnemyAiInRange(float _range, Vector3 _pos)
    {
        List<Enemy_Ai> inRange = new List<Enemy_Ai>();
        foreach(Enemy_Ai enemy in m_enemyAiList)
        {
            if(enemy != null)
            {
                if (Vector3.Distance(enemy.transform.position, _pos) <= _range) inRange.Add(enemy);
            }
        }
        return inRange;
    }

    private float m_playerHeight = 1.75f;
    public float GetPlayerHeight() { return m_playerHeight; }

    [SerializeField] private List<Lamp_Controller> m_lamps = new List<Lamp_Controller>();
    public List<Lamp_Controller> GetLamps() { return m_lamps; }
    public void AddLamp(Lamp_Controller _lamp)
    {
        if(m_lamps.Contains(_lamp))
        {

        }
        else
        {
            m_lamps.Add(_lamp);
        }
    }


    [SerializeField] private List<Vector4> m_lampVectors = new List<Vector4>();
    [SerializeField] private List<Vector4> m_lampColors = new List<Vector4>();
    [SerializeField] private int m_lampShaderArrayId;
    [SerializeField] private int m_lampColorsArrayId;

    public void ResetLamps(Lamp_Controller _lamp)
    {
        if (m_lamps.Count == 0)
        {
            m_lamps = new List<Lamp_Controller>();
        }
        else if(_lamp == m_lamps[0])
        {
            m_lamps = new List<Lamp_Controller>();
        }
    }
    public void ResetLamps(bool _force)
    {
        if (_force)
        {
            m_lamps = new List<Lamp_Controller>();
        }
    }
    public void UpdateShaderArray()
    {
        if (m_lamps.Count <= 0) return;
        ResetLampLists();
        CreateGlobalValues();
        int added = 0;
        for (int i = 0; i < MAX_LAMPS; i += 0)
        {
            if (i >= m_lamps.Count) break;
            if (m_lamps[i] != null)
            {
                Vector4 newVec = m_lamps[i].transform.position;
                newVec.w = m_lamps[i].GetNoisyEnabledRange();
                m_lampVectors[added] = newVec;
                m_lampColors[added] = m_lamps[i].LampColor;
                added++;
            }
            i++;
        }
        Shader.SetGlobalVectorArray("Lamps", m_lampVectors);
        Shader.SetGlobalVectorArray("Colors", m_lampColors);
    }

    public void ResetLampLists()
    {
        m_lampVectors = new List<Vector4>(MAX_LAMPS);
        m_lampColors = new List<Vector4>(MAX_LAMPS);
        for (int i = 0; i < MAX_LAMPS; i++)
        {
            m_lampVectors.Add(Vector4.zero);
            m_lampColors.Add(Color.black);
        }
    }

    [SerializeField] private bool m_globalValuesInitialised;
    public void CreateGlobalValues()
    {
        if (m_globalValuesInitialised) return;
        m_lampShaderArrayId = Shader.PropertyToID("Lamps");
        m_lampColorsArrayId = Shader.PropertyToID("Colors");
        Shader.SetGlobalVectorArray(m_lampShaderArrayId, m_lampVectors);
        Shader.SetGlobalVectorArray(m_lampColorsArrayId, m_lampVectors);
        m_globalValuesInitialised = true;
    }

    [SerializeField] private List<Ai_Waypoint> m_waypoints = new List<Ai_Waypoint>();
    public List<Ai_Waypoint> GetWaypoints() { return m_waypoints; }
    public void AddWaypoint(Ai_Waypoint _waypoint)
    {
        if(m_waypoints.Contains(_waypoint))
        {

        }
        else
        {
            m_waypoints.Add(_waypoint);
        }
    }

    private void Awake()
    {
        m_singleton = this;
        Enemy_Ai.ScaredWolf = false;
        Enemy_Ai.PlayerSpotted = false;
    }

    private void LateUpdate()
    {
        UpdateShaderArray();
    }

}
