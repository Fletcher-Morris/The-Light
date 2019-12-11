using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class escapevillage : MonoBehaviour
{
    public void EscapeTheVillage()
    {
        Player_Controller.Singleton().TeleportPlayer(new Vector3(249.65f, 25.31f, -191.28f), Vector3.forward, Vector2.zero);
    }
}
