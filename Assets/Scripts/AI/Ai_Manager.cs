using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Ai_Manager
{
    private static int m_lastAiId = -1;
    public static int GetNewAiId()
    {
        m_lastAiId++;
        return m_lastAiId;
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
    public static void UpdateShaderArray(Lamp_Controller _lamp)
    {
        if (m_lamps.Count <= 0) return;
        if(m_lamps[0] == _lamp)
        {
            m_lampPositions = new List<Vector4>();
            m_lampRanges = new List<float>();
            int i = 0;
            foreach(Lamp_Controller lamp in m_lamps)
            {
                if(lamp)
                {
                    m_lampPositions.Add(lamp.transform.position);
                    m_lampRanges.Add(lamp.GetNoisyEnabledRange());
                }
                else
                {
                    m_lamps.RemoveAt(i);
                    i--;
                }
                i++;
            }
            Shader.SetGlobalInt("LampCount", i);
            Shader.SetGlobalVectorArray("LampPositionsArray", m_lampPositions);
            Shader.SetGlobalFloatArray("LampRangesArray", m_lampRanges);
        }
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
