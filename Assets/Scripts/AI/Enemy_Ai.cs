using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy_Ai : MonoBehaviour
{
    [ReadOnly] [SerializeField] private int m_aiId;
    [SerializeField] private Ai_State m_aiState;
    [ReadOnly] [SerializeField] private Ai_State m_prevAiState;
    [SerializeField] private float m_stateCheckInterval = 1.0f;
    private float m_timeToStateCheck = 0.0f;

    [SerializeField] private UnityEvent OnDeath;


    private void Start()
    {
        m_aiId = Ai_Manager.GetNewAiId();
        StartCoroutine(StateCheckCoroutine());
    }

    private void Update()
    {
        
    }

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
            yield return new WaitForEndOfFrame();
        }
        Death();
        yield return null;
    }
    private void CheckAiState()
    {
        if (m_prevAiState != m_aiState) Debug.Log("Ai " + m_aiId + " State : " + m_aiState.ToString());
        m_prevAiState = m_aiState;
    }


    public void Kill()
    {
        m_aiState = Ai_State.Dead;
    }
    private void Death()
    {
        Debug.Log("Enemy " + m_aiId + " Died!");
    }

}