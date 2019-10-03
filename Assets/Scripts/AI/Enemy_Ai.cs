﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

[RequireComponent (typeof(Animator))]
[RequireComponent (typeof(NavMeshAgent))]
public class Enemy_Ai : MonoBehaviour
{
    //  The unique ID for this AI.
    private int m_aiId;
    //  The current state of this AI.
    [SerializeField] private Ai_State m_aiState;
    public Ai_State GetAiState() { return m_aiState; }
    //  The state of this AI during the previous update.
    private Ai_State m_prevAiState;
    //  The time between AI updates.
    [SerializeField] private float m_stateCheckInterval = 0.1f;
    //  The time until the next AI.
    private float m_timeToStateCheck = 0.0f;

    //  MEthods to be run upon the death of this AI.
    [SerializeField] private UnityEvent OnDeath;

    //  The Animator component for ths AI.
    private Animator m_animator;
    //  The NavMesh component for this AI.
    private NavMeshAgent m_navMeshAgent;
    //  The target position for the NavMesh.
    private Vector3 m_navTarget;

    //  The logic settings for this AI.
    [SerializeField] private Ai_Settings m_aiSettings;
    public Ai_Settings GetSettings() { return m_aiSettings; }

    //  Does this AI have a visual on the player?
    private bool m_playerVisible;
    //  The Transform of this AI's 'eyes'.
    [SerializeField] private Transform m_eyesTransform;
    //  Is this AI in light?
    private bool m_inLight;
    private Vector3 m_normalisedLightDir = new Vector3();
    private float m_fleeTime = 0.0f;

    //  The position where tihs AI originally spawned.
    private Vector3 m_spawnPos;
    //  Should this AI automatically gather waypoints?
    [SerializeField] private bool m_autoWaypoints = true;
    //  The list of waypoints this AI will try to visit.
    [SerializeField] private List<Ai_Waypoint> m_waypoints = new List<Ai_Waypoint>();
    [SerializeField] private Ai_Waypoint m_targetWaypoint;
    private Ai_Waypoint m_prevWaypoint;
    private int m_waypointPingPongDirection = 1;

    //  Is this AI in debug mode?
    [SerializeField] private bool m_debug;
    //  The original name of the GameObject.
    private string m_originalObjectName;

    private void Start()
    {
        m_aiId = Ai_Manager.GetNewAiId();
        GetComponentsOnStart();
        m_spawnPos = transform.position;
        if (m_autoWaypoints) GetAutoWaypoints();
    }

    //  A method for gathering components on Start.
    private void GetComponentsOnStart()
    {
        if (!m_animator) m_animator = GetComponent<Animator>();
        if (!m_navMeshAgent) m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_originalObjectName = transform.name;
        if (m_eyesTransform == null) m_eyesTransform = transform.Find("AI_EYES_TRANSFORM");
        if (m_eyesTransform == null) m_eyesTransform = transform;
    }

    private void GetAutoWaypoints()
    {
        foreach(Ai_Waypoint waypoint in Ai_Manager.GetWaypoints())
        {
            if(waypoint.GetExcludedAi().Contains(m_aiSettings))
            {

            }
            else
            {
                if(Vector3.Distance(waypoint.transform.position, m_spawnPos) <= m_aiSettings.wanderRadius)
                {
                    m_waypoints.Add(waypoint);
                }
            }
        }
    }

    private Ai_Waypoint GetNextWaypoint(Ai_Waypoint _current)
    {
        if(m_waypoints.Count <= 1)
        { return _current; }
        int currentIndex = m_waypoints.IndexOf(_current);
        Ai_Waypoint nextWaypoint = _current;
        if(m_waypoints.Count == 2)
        {
            if (currentIndex == 0) return m_waypoints[1];
            else return m_waypoints[0];
        }
        else
        {
            switch (m_aiSettings.waypointChoice)
            {
                case AiWaypointChoice.loop:
                    if (m_waypoints.Count == currentIndex + 1)
                    {
                        nextWaypoint = m_waypoints[0];
                    }
                    else
                    {
                        nextWaypoint = m_waypoints[currentIndex + 1];
                    }
                    break;
                case AiWaypointChoice.pingPong:
                    int t = currentIndex + m_waypointPingPongDirection;
                    t = Mathf.Clamp(t, 0, m_waypoints.Count - 1);
                    if (t == m_waypoints.Count - 1)
                    {
                        t--;
                        m_waypointPingPongDirection = -1;
                    }
                    else if (t == 0)
                    {
                        t++;
                        m_waypointPingPongDirection = 1;
                    }
                    nextWaypoint = m_waypoints[t];
                    break;
                case AiWaypointChoice.random:
                    List<Ai_Waypoint> randomList = m_waypoints;
                    randomList.Remove(_current);
                    nextWaypoint = randomList[Random.Range(0, randomList.Count - 1)];
                    break;
                case AiWaypointChoice.none:
                    nextWaypoint = _current;
                    break;
                default:
                    nextWaypoint = _current;
                    break;
            }
        }
        return nextWaypoint;
    }


    private void LateUpdate()
    {
        if (m_debug) DebugAi();
        else NoDebug();

        m_fleeTime -= Time.deltaTime;
        m_fleeTime = Mathf.Clamp(m_fleeTime, 0.0f, m_aiSettings.fleeDuration);
    }

    private void OnEnable()
    {
        m_timeToStateCheck = Random.Range(0.0f, m_stateCheckInterval);
        StartCoroutine(StateCheckCoroutine());
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }

    //  The main Update loop for this AI.
    private IEnumerator StateCheckCoroutine()
    {
        while(m_aiState != Ai_State.Dead)
        {
            if(m_timeToStateCheck <= 0.0f)
            {
                m_timeToStateCheck = m_stateCheckInterval;
                CheckAiState();
            }
            else
            {
                m_timeToStateCheck -= Time.deltaTime;
            }
            yield return null;
        }
        Death();
        yield return null;
    }

    //  Check the state of this AI, run logic, etc.
    private void CheckAiState()
    {
        CheckForPlayer();
        CheckForLight();
        CalcAiState();
        UpdateNavAgent();
        if (m_prevAiState != m_aiState)
        {
            switch (m_aiState)
            {
                case Ai_State.Idle:
                    SetAnimState("idle");
                    break;
                case Ai_State.Wandering:
                    SetAnimState("walking");
                    break;
                case Ai_State.Searching:
                    SetAnimState("walking");
                    break;
                case Ai_State.Chasing:
                    SetAnimState("running");
                    break;
                case Ai_State.Attacking:
                    SetAnimState("attacking");
                    break;
                case Ai_State.Stunned:
                    SetAnimState("stunned");
                    break;
                case Ai_State.Dead:
                    break;
                case Ai_State.Fleeing:
                    SetAnimState("running");
                    break;
                default:
                    break;
            }
            Debug.Log("Ai " + m_aiId + " State : " + m_aiState.ToString());
        }

        m_prevAiState = m_aiState;
    }

    //  Check if this AI can see the player.
    private void CheckForPlayer()
    {
        if (m_aiSettings.targetPlayer == false) return;
        Transform player = Ai_Manager.GetPlayerTransform();
        m_playerVisible = false;
        if(player)
        {
            Vector3 dir = player.position - m_eyesTransform.position;
            Ray ray = new Ray(m_eyesTransform.position, dir);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, Mathf.Infinity, LayerTools.AllLayers().RemoveLayers(new string[] { "Enemy", "Player" }));
            if (hit.collider)
            {
                if (hit.collider.transform == player)
                {
                    m_navTarget = player.position;
                    m_playerVisible = true;
                    m_aiState = Ai_State.Chasing;
                }
                else
                {
                    m_aiState = Ai_State.Wandering;
                }
            }
        }
    }

    private void CheckForLight()
    {
        m_inLight = false;
        m_normalisedLightDir = new Vector3();
        foreach(Lamp_Controller lamp in Ai_Manager.GetLamps())
        {
            Vector3 dir = m_eyesTransform.position - lamp.transform.position;
            Ray ray = new Ray(lamp.transform.position, dir);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, lamp.GetRange(), LayerTools.AllLayers().RemoveLayers(new string[] { "Enemy", "Player" }));
            if (hit.collider)
            {
                if (hit.collider.transform == transform)
                {
                    m_inLight = true;
                }
            }
            m_normalisedLightDir += dir;
        }
    }

    private void CalcAiState()
    {
        if(m_fleeTime > 0.0f)
        {
            m_aiState = Ai_State.Fleeing;
        }
        else
        {
            if (m_playerVisible)
            {
                m_aiState = Ai_State.Chasing;
            }
            else m_aiState = Ai_State.Wandering;
            if (m_inLight)
            {
                if (m_aiSettings.runAwayFromLight)
                {
                    m_aiState = Ai_State.Fleeing;
                    m_fleeTime = m_aiSettings.fleeDuration;
                }
            }
        }


        switch (m_aiState)
        {
            case Ai_State.Idle:
                m_navTarget = transform.position;
                break;
            case Ai_State.Wandering:
                m_navTarget = transform.position;
                break;
            case Ai_State.Searching:
                m_navTarget = Ai_Manager.GetPlayerTransform().position;
                break;
            case Ai_State.Chasing:
                m_navTarget = Ai_Manager.GetPlayerTransform().position;
                break;
            case Ai_State.Attacking:
                m_navTarget = Ai_Manager.GetPlayerTransform().position;
                break;
            case Ai_State.Stunned:
                m_navTarget = transform.position;
                break;
            case Ai_State.Dead:
                m_navTarget = transform.position;
                break;
            case Ai_State.Fleeing:
                m_navTarget = transform.position + (m_normalisedLightDir * m_aiSettings.fleeDistanceMultiplier);
                break;
            default:
                m_navTarget = Ai_Manager.GetPlayerTransform().position;
                break;
        }
    }

    //  Update the NavAgent with relavent data.
    private void UpdateNavAgent()
    {
        m_navMeshAgent.SetDestination(m_navTarget);
        switch (m_aiState)
        {
            case Ai_State.Idle:
                m_navMeshAgent.speed = m_aiSettings.wanderMoveSpeed;
                break;
            case Ai_State.Wandering:
                m_navMeshAgent.speed = m_aiSettings.wanderMoveSpeed;
                break;
            case Ai_State.Searching:
                m_navMeshAgent.speed = m_aiSettings.searchMoveSpeed;
                break;
            case Ai_State.Chasing:
                m_navMeshAgent.speed = m_aiSettings.chaseMoveSpeed;
                break;
            case Ai_State.Attacking:
                m_navMeshAgent.speed = m_aiSettings.chaseMoveSpeed;
                break;
            case Ai_State.Stunned:
                m_navMeshAgent.speed = 0.0f;
                break;
            case Ai_State.Dead:
                m_navMeshAgent.speed = 0.0f;
                break;
            case Ai_State.Fleeing:
                m_navMeshAgent.speed = m_aiSettings.chaseMoveSpeed;
                break;
            default:
                m_navMeshAgent.speed = m_aiSettings.wanderMoveSpeed;
                break;
        }
    }    

    //  Runs the various Debug methods.
    private void DebugAi()
    {
        if (m_playerVisible) Debug.DrawLine(m_eyesTransform.position, Ai_Manager.GetPlayerTransform().position);
        for(int i = 0; i < m_navMeshAgent.path.corners.Length; i++)
        {
            Vector3 currentPoint, nextPoint;
            if (i == 0) { currentPoint = transform.position; nextPoint = m_navMeshAgent.path.corners[0]; }
            else if(i == m_navMeshAgent.path.corners.Length) { currentPoint = m_navMeshAgent.path.corners[i]; nextPoint = m_navMeshAgent.destination; }
            else { currentPoint = m_navMeshAgent.path.corners[i-1]; nextPoint = m_navMeshAgent.path.corners[i]; }
            Debug.DrawLine(currentPoint, nextPoint, Color.blue);
        }

        foreach (Lamp_Controller lamp in Ai_Manager.GetLamps())
        {
            Debug.DrawLine(lamp.transform.position, m_eyesTransform.position, Color.red);
        }

        transform.name = m_originalObjectName + " [" + m_aiState.ToString() + "]";
    }
    private void NoDebug()
    {
        transform.name = m_originalObjectName;
    }


    // A method used to kill the AI from anyehere.
    public void Kill()
    {
        m_aiState = Ai_State.Dead;
    }
    //  What happens when the AI dies.
    private void Death()
    {
        SetAnimState("dead");
        Debug.Log("Enemy " + m_aiId + " Died!");
        OnDeath.Invoke();
    }

    //  Set the animation state of the AI animator.
    private void SetAnimState(string _state)
    {
        foreach(AnimatorControllerParameter param in m_animator.parameters)
        {
            if(param.type == AnimatorControllerParameterType.Bool)
            m_animator.SetBool(param.name, param.name == _state);
        }
    }

}