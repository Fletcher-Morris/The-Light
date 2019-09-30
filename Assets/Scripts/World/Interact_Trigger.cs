using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Interact_Trigger : MonoBehaviour
{
    [SerializeField] private float m_interactDistance = 2.0f;
    public float InteractionDistance() { return m_interactDistance; }

    [SerializeField] private bool m_isClosest;
    public void SetAsClosest(bool _closest) { m_isClosest = _closest; }

    [SerializeField] private string m_text = "Interact";

    [SerializeField] private UnityEvent OnInteract;

    private Player_Controller m_player;

    private void Start()
    {
        m_player = Ai_Manager.GetPlayerTransform().GetComponent<Player_Controller>();
        m_player.AddInteraction(this);
    }

    private void Update()
    {
        if(m_isClosest)
        {

        }
    }

    public void TriggerInteraction()
    {
        Debug.Log("Interaction Triggered");
        OnInteract.Invoke();
    }
}
