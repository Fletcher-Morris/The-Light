using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    private void Awake()
    {
        Ai_Manager.SetPlayerTransform(transform);
    }
}