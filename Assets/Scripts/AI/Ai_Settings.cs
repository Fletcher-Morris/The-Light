using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New AI", menuName = "AI Settings", order = 1)]
[System.Serializable]
public class Ai_Settings : ScriptableObject
{
    //  Should the AI target the player?
    public bool targetPlayer = true;

    [Header("Distance Settings")]
    //  The range at which the AI will stop wandering.
    public float interestRange = 30.0f;
    //  The rangeat which the AI woll chase the player.
    public float chaseRange = 20.0f;
    //  The range at which the AI will stop chasing the player.
    public float stopChaseRange = 30.0f;
    //  The range at which the AI will try to attack the player.
    public float attackRange = 1.0f;

    [Header("Speed Settings")]
    //  The speed at which the AI moves when wandering around.
    public float wanderMoveSpeed = 3.0f;
    //  The speed at which the AI moves when searching for the player.
    public float searchMoveSpeed = 6.0f;
    //  The speed at which the AI moves when chasing the player.
    public float chaseMoveSpeed = 10.0f;
    //  The speed at which the AI moves when fleeing a threat.
    public float fleeMoveSpeed = 10.0f;

    [Header("Vision Settings")]
    //  The layers that obstruct the AI's vision.
    public LayerMask visionObstructors = LayerTools.AllLayers();

    [Header("Behaviour Settings")]
    //  Should the AI be battracted to lamps?
    public bool attractedToLight = false;
    //  Should the AI flee from lamps?
    public bool runAwayFromLight = true;
    //  The minumum time the AI will flee for.
    public float fleeDuration = 3.0f;
    //  How many far the AI will run away from the danger.
    public float fleeDistanceMultiplier = 3.0f;

    [Header("Patrol Settings")]
    public AiWaypointChoice waypointChoice = AiWaypointChoice.loop;
    public float waypointTollerance = 1.0f;
    public float wanderRadius = 30.0f;
}

public enum AiWaypointChoice
{
    loop,
    pingPong,
    random,
    none
}
