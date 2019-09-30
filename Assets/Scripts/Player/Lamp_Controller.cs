using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class Lamp_Controller : MonoBehaviour
{
    [SerializeField] private float m_range = 7.5f;
    public float GetRange() { return m_range; }

    [SerializeField] bool m_mainLamp = true;

    public Shader m_shader;

    private void Start()
    {
        Ai_Manager.AddLamp(this);
    }

    private void Update()
    {
        if(m_mainLamp)
        {
            Shader.SetGlobalFloat("LampRange", m_range);
            Shader.SetGlobalVector("LampPosition", transform.position);
        }
    }
}
