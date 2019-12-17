using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reset_Lamps : MonoBehaviour
{
    public void ResetLamps()
    {
        if (Ai_Manager.Singleton() == null) return;
        Ai_Manager.Singleton().ResetLamps(true);
        Ai_Manager.Singleton().ResetLampLists();
        Ai_Manager.Singleton().CreateGlobalValues();
    }
}
