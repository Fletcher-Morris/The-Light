using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class Lamp_Controller : MonoBehaviour
{
    [SerializeField] private float m_range = 7.5f;
    public float GetRange()
    {
        return m_range;
    }

    [SerializeField] bool m_mainLamp = false;

    [SerializeField] private float m_noiseScale = 1.0f;
    [SerializeField] private List<AnimationCurve> m_noiseCurves;

    [SerializeField] private bool m_on = true;
    public bool IsOn() { return m_on; }

    private float m_animTime = 0.0f;


    [SerializeField] private float m_rotationLerp = 10.0f;

    private void Start()
    {
        Ai_Manager.AddLamp(this);
    }

    private float GetEnabledRange()
    {
        if (!m_on) return 0.0f;
        return GetRange();
    }

    public float GetNoisyRange()
    {
        float newRange = GetRange();
        foreach (AnimationCurve curve in m_noiseCurves)
        {
            newRange += (curve.Evaluate(m_animTime) * m_noiseScale);
        }
        return newRange;
    }

    public float GetNoisyEnabledRange()
    {
        if (!m_on) return 0.0f;
        return GetNoisyRange();
    }

    private void Update()
    {
        m_animTime += GameTime.deltaTime;

        if (m_mainLamp)
        {
            Shader.SetGlobalFloat("LampRange", GetNoisyEnabledRange());
            Shader.SetGlobalVector("LampPosition", transform.position);
        }

        if(m_rotationLerp > 0.0f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, new Quaternion(0, 0, 0, 1), m_rotationLerp * GameTime.deltaTime);
        }

    }

    public void Toggle(bool _on)
    {
        m_on = _on;
    }
}
