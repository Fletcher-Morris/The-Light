using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameTime
{
    //  Is the game currently paused?
    private static bool m_paused = false;
    //  Return the paused state of the game.
    public static bool IsPaused() { return m_paused; }
    //  Set the paused state of the game.
    public static void SetPaused(bool _pause) { m_paused = _pause; }
    //  Pause the game.
    public static void Pause() { m_paused = true; }
    //  Unpause the game.
    public static void UnPause() { m_paused = false; }


    //  Return deltaTime, or 0.0f if paused.
    public static float deltaTime { get { return m_paused ? 0.0f : Time.deltaTime; } }
    //  Return deltaTime, or 0.0f if paused.
    //public static float deltaTime { get { return m_paused ? 0.0f : Time.deltaTime; } }
}
