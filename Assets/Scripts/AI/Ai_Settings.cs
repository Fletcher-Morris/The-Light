using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ai_Settings
{
    public bool targetPlayer = true;
    public float interestRange = 30.0f;
    public float chaseRange = 20.0f;
    public float stopChaseRange = 30.0f;
}
