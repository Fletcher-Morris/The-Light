using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue_Controller : MonoBehaviour
{
    [SerializeField] private Dialogue m_dialogue;
    [SerializeField] private bool m_pauseGame = false;

    private Player_Controller m_player;

    private void Start()
    {
        m_player = Ai_Manager.GetPlayerTransform().GetComponent<Player_Controller>();
    }

    public void ActivateDialogue()
    {
        m_player.DisplayDialogue(m_dialogue, m_pauseGame);
    }
}
