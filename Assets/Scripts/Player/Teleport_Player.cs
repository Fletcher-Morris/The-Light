using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport_Player : MonoBehaviour
{
    [SerializeField] private Vector3 m_position;
    [SerializeField] private Vector3 m_direction;
    [SerializeField] private Vector2 m_cameraAngle;

    public void Teleport()
    {
        Player_Controller.Singleton()?.TeleportPlayer(m_position, m_direction, m_cameraAngle);
    }
}
