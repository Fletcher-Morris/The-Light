using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ai_Settings
{
    public bool targetPlayer = true;
    [Header("Distance Settings")]
    public float interestRange = 30.0f;
    public float chaseRange = 20.0f;
    public float stopChaseRange = 30.0f;
    [Header("Speed Settings")]
    public float wanderMoveSpeed = 3.0f;
    public float searchMoveSpeed = 6.0f;
    public float chaseMoveSpeed = 10.0f;
    public float fleeMoveSpeed = 10.0f;
    [Header("Vision Settings")]
    public Transform eyesTransform;
}
