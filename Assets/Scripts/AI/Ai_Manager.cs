﻿using System.Collections;
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

}
