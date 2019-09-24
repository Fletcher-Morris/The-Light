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
}
