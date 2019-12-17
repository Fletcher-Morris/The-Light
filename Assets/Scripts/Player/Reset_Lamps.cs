using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset_Lamps : MonoBehaviour
{
    public void ResetLamps()
    {
        Ai_Manager.ResetLamps(true);
        Ai_Manager.ResetLampLists();
        Ai_Manager.CreateGlobalValues();
    }
}
