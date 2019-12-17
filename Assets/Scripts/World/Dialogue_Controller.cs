using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue_Controller : MonoBehaviour
{
    [SerializeField] private Dialogue m_dialogue;
    [SerializeField] private bool m_pauseGame = false;

    private Player_Controller m_player;

    public void ActivateDialogue()
    {
        Player_Controller.Singleton().DisplayDialogue(m_dialogue, m_pauseGame);
    }
}
