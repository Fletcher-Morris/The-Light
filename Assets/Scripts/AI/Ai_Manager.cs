using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Ai_Manager
{
    const int MAX_LAMPS = 16;

    private static int m_lastAiId = -1;
    public static int GetNewAiId()
    {
        m_lastAiId++;
        return m_lastAiId;
    }

    private static List<Enemy_Ai> m_enemyAiList = new List<Enemy_Ai>();
    public static int AddAi(Enemy_Ai _ai)
    {
        m_enemyAiList.Add(_ai);
        return GetNewAiId();
    }

    public static List<Enemy_Ai> GetEnemyAiInRange(float _range, Vector3 _pos)
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

    private static Transform m_playerTransform;
    public static Transform GetPlayerTransform() { return m_playerTransform; }
    public static void SetPlayerTransform(Transform _player) { m_playerTransform = _player; }

    private static float m_playerHeight = 1.75f;
    public static float GetPlayerHeight() { return m_playerHeight; }

    private static List<Lamp_Controller> m_lamps = new List<Lamp_Controller>();
    public static List<Lamp_Controller> GetLamps() { return m_lamps; }
    public static void AddLamp(Lamp_Controller _lamp)
    {
        if(m_lamps.Contains(_lamp))
        {

        }
        else
        {
            m_lamps.Add(_lamp);
        }
    }


    private static List<Vector4> m_lampPositions = new List<Vector4>();
    private static List<float> m_lampRanges = new List<float>();

    public static void ResetLamps(Lamp_Controller _lamp)
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
    public static void UpdateShaderArray(Lamp_Controller _lamp)
    {
        if (m_lamps.Count <= 0) return;
        if(m_lamps[0] == _lamp)
        {
            ResetLampLists();
            CreateGlobalValues();
            int added = 0;
            for(int i = 0; i < MAX_LAMPS; i+=0)
            {
                if (i >= m_lamps.Count) break;
                if(m_lamps[i] != null)
                {
                    m_lampPositions[added] = m_lamps[i].transform.position;
                    m_lampRanges[added] = m_lamps[i].GetNoisyEnabledRange();
                    added++;
                }
                i++;
            }
            Shader.SetGlobalVectorArray("LampPositionsArray", m_lampPositions);
            Shader.SetGlobalFloatArray("LampRangesArray", m_lampRanges);
        }
    }

    private static void ResetLampLists()
    {
        m_lampPositions = new List<Vector4>(MAX_LAMPS);
        m_lampRanges = new List<float>(MAX_LAMPS);
        for (int i = 0; i < MAX_LAMPS; i++)
        {
            m_lampPositions.Add(Vector4.zero);
            m_lampRanges.Add(0.0f);
        }
    }

    private static bool m_globalValuesInitialised;
    private static void CreateGlobalValues()
    {
        if (m_globalValuesInitialised) return;
        Shader.SetGlobalInt("LampCount", MAX_LAMPS);
        Shader.SetGlobalVectorArray("LampPositionsArray", m_lampPositions);
        Shader.SetGlobalFloatArray("LampRangesArray", m_lampRanges);
        m_globalValuesInitialised = true;
    }

    private static List<Ai_Waypoint> m_waypoints = new List<Ai_Waypoint>();
    public static List<Ai_Waypoint> GetWaypoints() { return m_waypoints; }
    public static void AddWaypoint(Ai_Waypoint _waypoint)
    {
        if(m_waypoints.Contains(_waypoint))
        {

        }
        else
        {
            m_waypoints.Add(_waypoint);
        }
    }

}
