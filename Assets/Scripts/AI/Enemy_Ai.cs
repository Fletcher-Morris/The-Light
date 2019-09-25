﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

[RequireComponent (typeof(Animator))]
[RequireComponent (typeof(NavMeshAgent))]
public class Enemy_Ai : MonoBehaviour
{
    [ReadOnly] [SerializeField] private int m_aiId;
    [SerializeField] private Ai_State m_aiState;
    [ReadOnly] [SerializeField] private Ai_State m_prevAiState;
    [SerializeField] private float m_stateCheckInterval = 1.0f;
    private float m_timeToStateCheck = 0.0f;

    [SerializeField] private UnityEvent OnDeath;

    private Animator m_animator;
    private NavMeshAgent m_navMeshAgent;

    [SerializeField] private Ai_Settings m_aiSettings;

    [ReadOnly] [SerializeField] private bool m_playerVisible;
    [SerializeField] private bool m_debug;


    private void Start()
    {
        m_aiId = Ai_Manager.GetNewAiId();
        GetComponentsOnStart();
        StartCoroutine(StateCheckCoroutine());
    }

    private void GetComponentsOnStart()
    {
        if (!m_animator) m_animator = GetComponent<Animator>();
        if (!m_navMeshAgent) m_navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                m_navMeshAgent.SetDestination(hit.point);
            }
        }
    }

    private IEnumerator StateCheckCoroutine()
    {
        m_timeToStateCheck = Random.Range(0.0f, m_stateCheckInterval);
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
    private void CheckAiState()
    {
        CheckForPlayer();
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

    private void CheckForPlayer()
    {
        if (m_aiSettings.targetPlayer == false) return;
        Transform player = Ai_Manager.GetPlayerTransform();
        m_playerVisible = false;
        if(player)
        {
            Vector3 dir = player.position - transform.position;
            Ray ray = new Ray(transform.position, dir);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, Mathf.Infinity, LayerTools.AllLayers().RemoveLayer("Enemy"));
            if(hit.collider)
            {
                if (hit.collider.transform == player)
                {
                    m_navMeshAgent.SetDestination(player.position);
                    m_playerVisible = true;
                    m_aiState = Ai_State.Chasing;
                    if(m_debug) Debug.DrawLine(transform.position, hit.point, Color.red);
                }
                else
                {
                    m_aiState = Ai_State.Wandering;
                    if(m_debug) Debug.DrawLine(transform.position, hit.point, Color.yellow);
                }
            }
        }
    }

    private void UpdateNavAgent()
    {
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


    public void Kill()
    {
        m_aiState = Ai_State.Dead;
    }
    private void Death()
    {
        SetAnimState("dead");
        Debug.Log("Enemy " + m_aiId + " Died!");
    }

    private void SetAnimState(string _state)
    {
        foreach(AnimatorControllerParameter param in m_animator.parameters)
        {
            if(param.type == AnimatorControllerParameterType.Bool)
            m_animator.SetBool(param.name, param.name == _state);
        }
    }

}