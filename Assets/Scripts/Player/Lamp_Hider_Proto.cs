using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp_Hider_Proto : MonoBehaviour
{
    [SerializeField] private bool m_isHidingLamp = false;

    private Player_Controller m_player;

    [SerializeField] private Material m_lampLightMaterial;
    [SerializeField] private Material m_lampParticleMaterial;

    private bool m_prevState = false;

    private void Update()
    {
        if(m_prevState != PlayerInput.Hide)
        {
            if (PlayerInput.Hide == true)
            {
                m_lampLightMaterial.SetColor("_Color", Color.black);
                m_lampParticleMaterial.DisableKeyword("_EMISSION");
            }
            else
            {
                m_lampLightMaterial.SetColor("_Color", new Color(1.0f,0.8f,0.0f));
                m_lampParticleMaterial.EnableKeyword("_EMISSION");

            }
        }

        m_prevState = PlayerInput.Hide;
    }
}
