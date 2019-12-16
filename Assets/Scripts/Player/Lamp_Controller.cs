using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[ExecuteInEditMode]
public class Lamp_Controller : MonoBehaviour
{

    [Header("LAMP SETTINGS")]


    [SerializeField] private float m_range = 7.5f;
    public float GetRange()
    {
        return m_range;
    }

    [SerializeField] private float m_noiseScale = 1.0f;
    [SerializeField] private List<AnimationCurve> m_noiseCurves;

    [SerializeField] private bool m_on = true;
    public bool IsOn() { return m_on; }

    private float m_animTime = 0.0f;


    [SerializeField] private float m_rotationLerp = 10.0f;

    public Color LampColor = Color.white;

    private void Awake()
    {
        Ai_Manager.ResetLamps(this);
    }

    private void Start()
    {
        Ai_Manager.AddLamp(this);
    }

    public float GetEnabledRange()
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

        if(m_rotationLerp > 0.0f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, new Quaternion(0, 0, 0, 1), m_rotationLerp * GameTime.deltaTime);
        }

        if (m_repeatTime == 0.0f) return;
        m_timer -= GameTime.deltaTime;
        if (m_timer <= 0.0f)
        {
            m_timer = m_repeatTime;
            foreach (Powder powder in m_powders) UsePowder(powder);
        }
    }

    private void LateUpdate()
    {
        Ai_Manager.UpdateShaderArray(this);
    }

    public void Toggle(bool _on)
    {
        m_on = _on;
    }



    [Header("POWDER USAGE")]


    [SerializeField] private float m_repeatTime = 0.0f;
    [SerializeField] private List<Powder> m_powders = new List<Powder>();
    public void UsePowder(Powder _powder)
    {
        int m = 0;
        if(_powder.AffectsMonsters)
        {
            foreach (Enemy_Ai enemy in Ai_Manager.GetEnemyAiInRange(GetEnabledRange(), transform.position))
            {
                if(enemy != null)
                {
                    enemy.AddPowderEffect(_powder);
                    m++;
                }
            }
        }

        if(m > 0) Debug.Log("Used '" + _powder.GetName() + "' on " + m + " monsters.");
    }

    private float m_timer = 0.0f;
}
