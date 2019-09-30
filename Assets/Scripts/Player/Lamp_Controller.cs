using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class Lamp_Controller : MonoBehaviour
{
    [SerializeField] private float m_range = 7.5f;
    public float GetRange() { return m_range; }

    [SerializeField] bool m_mainLamp = true;

    [SerializeField] private float m_noiseScale = 1.0f;
    [SerializeField] private List<AnimationCurve> m_noiseCurves;

    private void Start()
    {
        Ai_Manager.AddLamp(this);
    }

    private void Update()
    {
        float newRange = m_range;
        foreach(AnimationCurve curve in m_noiseCurves)
        {
            newRange += (curve.Evaluate(Time.time) * m_noiseScale);
        }

        if(m_mainLamp)
        {
            Shader.SetGlobalFloat("LampRange", newRange);
            Shader.SetGlobalVector("LampPosition", transform.position);
        }
    }
}
