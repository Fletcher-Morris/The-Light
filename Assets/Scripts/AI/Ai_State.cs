using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum Ai_State
{
    Idle,
    Wandering,
    Searching,
    Chasing,
    Attacking,
    Stunned,
    Dead,
    Fleeing
}
