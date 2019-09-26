using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.AI;

[RequireComponent (typeof(Animator))]
[RequireComponent (typeof(NavMeshAgent))]
public class Enemy_Ai : MonoBehaviour
{
    //  The unique ID for this AI.
    [ReadOnly] [SerializeField] private int m_aiId;
    //  The current state of this AI.
    [SerializeField] private Ai_State m_aiState;
    //  The state of this AI during the previous update.
    [ReadOnly] [SerializeField] private Ai_State m_prevAiState;
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
    [ReadOnly] [SerializeField] private Vector3 m_navTarget;

    //  The logic settings for this AI.
    [SerializeField] private Ai_Settings m_aiSettings;

    //  Does this AI have a visual on the player?
    [ReadOnly] [SerializeField] private bool m_playerVisible;
    //  The Transform of this AI's 'eyes'.
    [SerializeField] private Transform m_eyesTransform;

    //  Is this AI in debug mode?
    [SerializeField] private bool m_debug;
    //  The original name of the GameObject.
    [ReadOnly] [SerializeField] private string m_originalObjectName;


    private void Start()
    {
        m_aiId = Ai_Manager.GetNewAiId();
        GetComponentsOnStart();
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

    private void LateUpdate()
    {
        if (m_debug) DebugAi();
        else NoDebug();
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
            Physics.Raycast(ray, out hit, Mathf.Infinity, LayerTools.AllLayers().RemoveLayer("Enemy"));
            if(hit.collider)
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