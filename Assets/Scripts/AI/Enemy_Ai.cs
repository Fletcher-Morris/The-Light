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
    private int m_aiId;
    //  The current state of this AI.
    [SerializeField] private Ai_State m_aiState;
    public Ai_State GetAiState() { return m_aiState; }
    //  The state of this AI during the previous update.
    private Ai_State m_prevAiState;
    //  The time between AI updates.
    [SerializeField] private float m_updateInterval = 0.1f;
    //  The time until the next AI.
    private float m_timeToStateCheck = 0.0f;

    //  Methods to be run upon the death of this AI.
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
    private float m_fleeTime = 0.0f;
    private float m_playerAttention = 0.0f;
    private Vector3 m_lightDirection = new Vector3();
    private float m_lightLevel;
    //  The change in light levels observed by this AI.
    private float m_lightLevelDelta;
    //  The light level obeserved during the previous frame.
    private float m_prevLightLevel;
    //  Is this AI surpsise4d by the current light level?
    private bool m_surprised = false;
    //  The last known position of the player.
    private Vector3 m_lastKnownPlayerPosition;
    //  The detection value of the player, 1.0 is fully detected.
    private float m_playerDetectionValue = 0.0f;
    //  The layers that obstruct light.
    private LayerMask m_lampTestMask;

    //  The position where tihs AI originally spawned.
    private Vector3 m_spawnPos;
    //  Should this AI automatically gather waypoints?
    [SerializeField] private bool m_autoWaypoints = true;
    //  The list of waypoints this AI will try to visit.
    [SerializeField] private List<Ai_Waypoint> m_waypoints = new List<Ai_Waypoint>();
    [SerializeField] private Ai_Waypoint m_targetWaypoint;
    private Ai_Waypoint m_prevWaypoint;
    private int m_waypointPingPongDirection = 1;

    //  This AI's audio source.
    private AudioSource m_audioSource;

    //  Is this AI in debug mode?
    [SerializeField] private bool m_debug;
    //  The original name of the GameObject.
    private string m_originalObjectName;
    //  Is this AI enabled?
    [SerializeField] private bool m_aiEnabled = true; 

    private void Start()
    {
        m_aiId = Ai_Manager.GetNewAiId();
        GetComponentsOnStart();
        m_spawnPos = transform.position;
        if (m_autoWaypoints) GetAutoWaypoints();
        m_lampTestMask = LayerTools.CreateLayerMask(new string[]{ "Default", "Ground", "Terrain", "Lamp"});
    }

    //  Enable the AI.
    public void EnableAi()
    {
        m_aiEnabled = true;
    }
    //  Disable the AI.
    public void DisableAi()
    {
        m_aiEnabled = true;
    }
    //  Set the enabled state.
    public void SetAiEnabled(bool _enabled)
    {
        m_aiEnabled = _enabled;
    }
    //  Toggle the enabled state.
    public void ToggleAiEnabled()
    {
        m_aiEnabled = !m_aiEnabled;
    }

    //  A method for gathering components on Start.
    private void GetComponentsOnStart()
    {
        if (!m_animator) m_animator = GetComponent<Animator>();
        if (!m_navMeshAgent) m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_originalObjectName = transform.name;
        if (m_eyesTransform == null) m_eyesTransform = transform.Find("AI_EYES_TRANSFORM");
        if (m_eyesTransform == null) m_eyesTransform = transform;
        m_audioSource = GetComponent<AudioSource>();
    }

    private void GetAutoWaypoints()
    {
        foreach(Ai_Waypoint waypoint in Ai_Manager.GetWaypoints())
        {
            if(waypoint.GetIncludedAi().Contains(m_aiSettings))
            {
                if(Vector3.Distance(waypoint.transform.position, m_spawnPos) <= m_aiSettings.wanderRadius)
                {
                    m_waypoints.Add(waypoint);
                }
            }
            else
            {
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
                        m_waypointPingPongDirection = -1;
                    }
                    else if (t == 0)
                    {
                        m_waypointPingPongDirection = 1;
                    }
                    nextWaypoint = m_waypoints[t];
                    break;
                case AiWaypointChoice.random:
                    List<Ai_Waypoint> randomList = new List<Ai_Waypoint>(m_waypoints);
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
        if (m_navMeshAgent.CalculatePath(nextWaypoint.transform.position, new NavMeshPath()) == false)
        {
            Debug.LogWarning("Waypoint '" + nextWaypoint.transform.name + "' is unreachable by AI:" + m_aiId + " '" + m_originalObjectName + "'!");
            return GetNextWaypoint(nextWaypoint);
        }
        return nextWaypoint;
    }

    private Vector3 GetBestWaypointPos(Vector3 input)
    {
        if (m_waypoints.Count == 0) return input;
        else if(m_targetWaypoint != null)
        {
            if (Vector3.Distance(m_targetWaypoint.transform.position, transform.position) <= m_aiSettings.waypointTollerance)
            {
                m_prevWaypoint = m_targetWaypoint;
                m_targetWaypoint = GetNextWaypoint(m_prevWaypoint);
            }
        }
        else
        {
            if(m_aiSettings.randomStartWaypoint)
            {
                m_targetWaypoint = m_waypoints[Random.Range(0, m_waypoints.Count - 1)];
            }
            else
            {
                float closestDist = Mathf.Infinity;
                Ai_Waypoint closestWaypoint = null;
                foreach (Ai_Waypoint waypoint in m_waypoints)
                {
                    float dist = Vector3.Distance(waypoint.transform.position, transform.position);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closestWaypoint = waypoint;
                    }
                }
                m_targetWaypoint = closestWaypoint;
            }
        }
        if (m_targetWaypoint == null) return m_spawnPos;
        else return m_targetWaypoint.transform.position;
    }


    private void LateUpdate()
    {
        if (m_debug) DebugAi();
        else NoDebug();

        m_fleeTime -= GameTime.deltaTime;
        m_fleeTime = Mathf.Clamp(m_fleeTime, 0.0f, m_aiSettings.fleeDuration);

        m_playerAttention -= GameTime.deltaTime;
        m_playerAttention = Mathf.Clamp(m_playerAttention, 0.0f, m_aiSettings.attentionSpan);
    }

    private void OnEnable()
    {
        m_timeToStateCheck = Random.Range(0.0f, m_updateInterval);
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
                m_timeToStateCheck = m_updateInterval;
                CheckAiState();
            }
            else
            {
                m_timeToStateCheck -= GameTime.deltaTime;
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
                    if(m_audioSource)
                    m_audioSource.PlayOneShot(Audio_Manager.Singleton().wolfHowl);
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
            Vector3 dir;
            Ray ray;
            RaycastHit playerHit = new RaycastHit();
            bool hitPlayer = false;
            int tries = 4;
            for(int i = 0; i < tries; i++)
            {
                if (hitPlayer) break;
                dir = (player.position + new Vector3(0, Ai_Manager.GetPlayerHeight() * (i+1 / tries), 0)) - m_eyesTransform.position;
                ray = new Ray(m_eyesTransform.position, dir);
                hitPlayer = false;
                Physics.Raycast(ray, out playerHit, m_aiSettings.interestRange, m_aiSettings.visionObstructors);
                if (playerHit.collider)
                {
                    if (playerHit.collider.CompareTag("Player"))
                    {
                        hitPlayer = true;
                    }
                }
            }
            if (hitPlayer)
            {
                m_playerVisible = true;
                if (m_playerDetectionValue >= 1.0f)
                {
                    //  Player detected
                    m_navTarget = player.position;
                    m_aiState = Ai_State.Chasing;
                    m_lastKnownPlayerPosition = Ai_Manager.GetPlayerTransform().position;
                    m_playerAttention = m_aiSettings.attentionSpan;
                }
                else
                {
                    //  Player undetected
                    m_playerDetectionValue += m_aiSettings.detectionCurve.Evaluate(playerHit.distance / m_aiSettings.interestRange) * m_updateInterval;
                }
                if (m_debug) Debug.DrawLine(m_eyesTransform.position, playerHit.point, Color.Lerp(Color.green, Color.red, m_playerDetectionValue), m_updateInterval);
            }
            else if (m_playerAttention > 0.0f)
            {
                m_playerDetectionValue -= m_updateInterval;
            }
            else
            {
                m_playerDetectionValue = 0.0f;
                m_aiState = Ai_State.Wandering;
            }
            m_playerDetectionValue = Mathf.Clamp01(m_playerDetectionValue);
        }
    }

    private void CheckForLight()
    {
        m_inLight = false;
        m_lightDirection = new Vector3();
        m_prevLightLevel = m_lightLevel;
        m_lightLevel = 0.0f;
        foreach(Lamp_Controller lamp in Ai_Manager.GetLamps())
        {
            if(lamp.IsOn())
            {
                Vector3 dir = lamp.transform.position - m_eyesTransform.position;
                Ray ray = new Ray(m_eyesTransform.position, dir);
                RaycastHit hit;
                Physics.Raycast(ray, out hit, lamp.GetRange(), m_lampTestMask);
                if(hit.collider)
                {
                    if (hit.collider.transform == lamp.transform)
                    {
                        m_inLight = true;
                        m_lightLevel += Mathf.Clamp(1 - (dir.magnitude / lamp.GetRange()), 0.0f, Mathf.Infinity);
                        if (m_debug) Debug.DrawLine(lamp.transform.position, m_eyesTransform.position, Color.blue, m_updateInterval);
                    }
                }
                m_lightDirection += dir;
            }
        }
        m_lightLevelDelta = (m_lightLevel - m_prevLightLevel);
        m_lightLevelDelta = Mathf.Clamp(m_lightLevelDelta, 0.0f, Mathf.Infinity);
        m_surprised = ((m_lightLevelDelta * m_aiSettings.surpriseMultiplier * m_updateInterval) > m_aiSettings.surpriseThreshold);
    }

    private void CalcAiState()
    {
        //  Check if the AI is enabled.
        if(m_aiEnabled == false)
        {
            m_navTarget = transform.position;
            return;
        }

        if(m_fleeTime > 0.0f)
        {
            m_aiState = Ai_State.Fleeing;
        }
        else
        {
            if(m_playerAttention > 0.0f)
            {
                if (m_playerVisible)
                {
                    m_aiState = Ai_State.Chasing;
                }
                else
                {
                    m_aiState = Ai_State.Searching;
                }
            }
            else m_aiState = Ai_State.Wandering;
            if ((m_inLight && m_aiSettings.runAwayFromLight) || m_surprised)
            {
                m_aiState = Ai_State.Fleeing;
                m_fleeTime = m_aiSettings.fleeDuration;
            }
        }


        switch (m_aiState)
        {
            case Ai_State.Idle:
                m_navTarget = transform.position;
                break;
            case Ai_State.Wandering:
                m_navTarget = GetBestWaypointPos(transform.position);
                break;
            case Ai_State.Searching:
                m_navTarget = m_lastKnownPlayerPosition;
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
                m_navTarget = transform.position + (m_lightDirection * m_aiSettings.fleeDistanceMultiplier);
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
        //if (m_playerVisible) Debug.DrawLine(m_eyesTransform.position, (Ai_Manager.GetPlayerTransform().position + new Vector3(0, Ai_Manager.GetPlayerHeight() / 2.0f, 0)));
        for(int i = 0; i < m_navMeshAgent.path.corners.Length; i++)
        {
            Vector3 currentPoint, nextPoint;
            if (i == 0) { currentPoint = transform.position; nextPoint = m_navMeshAgent.path.corners[0]; }
            else if(i == m_navMeshAgent.path.corners.Length) { currentPoint = m_navMeshAgent.path.corners[i]; nextPoint = m_navMeshAgent.destination; }
            else { currentPoint = m_navMeshAgent.path.corners[i-1]; nextPoint = m_navMeshAgent.path.corners[i]; }
            Debug.DrawLine(currentPoint, nextPoint, Color.magenta);
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