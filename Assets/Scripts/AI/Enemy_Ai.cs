using System.Collections;
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
        Debug.Log("A");
        if (m_aiSettings.targetPlayer == false) return;

        Debug.Log("B");
        Transform player = Ai_Manager.GetPlayerTransform();
        if(player)
        {
            Debug.Log("C");
            Vector3 dir = player.position - transform.position;
            Ray ray = new Ray(transform.position, dir);
            RaycastHit hit;
            Physics.Raycast(ray, out hit);
            if(hit.collider.transform == player)
            {
                Debug.Log("D");
                m_navMeshAgent.SetDestination(player.position);
                m_playerVisible = true;
            }
            else
            {
                m_playerVisible = false;
            }
        }
        else
        {
            m_playerVisible = false;
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