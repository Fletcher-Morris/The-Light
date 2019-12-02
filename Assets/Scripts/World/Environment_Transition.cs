using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Environment_Transition : MonoBehaviour
{

    [SerializeField] private EnvironmentSettings m_initialEnvironment;
    [SerializeField] private EnvironmentSettings m_currentEnvironment;
    [SerializeField] private float m_transitionTime = 10.0f;
    [SerializeField] private bool m_isTransitioning = false;
    [SerializeField] private Light m_mainLight;
    [SerializeField] private Material m_skyMaterial;
    [SerializeField] private ParticleSystem m_rain;

    private int m_ambientColorProperty;
    private int m_cloudColorProperty;

    private void Start()
    {
        m_ambientColorProperty = Shader.PropertyToID("AmbientColor");
        m_cloudColorProperty = Shader.PropertyToID("CloudColor");

        if (m_mainLight == null) m_mainLight = GameObject.Find("Directional Light").GetComponent<Light>();

        if (m_initialEnvironment)
        {
            m_currentEnvironment = m_initialEnvironment;
            Transition(m_initialEnvironment, 0.5f);
        }
    }

    public void Transition(EnvironmentSettings _settings)
    {
        if (m_isTransitioning == false) StartCoroutine(TransitionCoroutine(_settings, m_transitionTime));
    }
    public void Transition(EnvironmentSettings _settings, float _time)
    {
        if (m_isTransitioning == false) StartCoroutine(TransitionCoroutine(_settings, _time));
    }

    private IEnumerator TransitionCoroutine(EnvironmentSettings _environment, float _time)
    {
        m_isTransitioning = true;

        float t = 0.0f;
        float lerp = 0.0f;

        while(t < _time)
        {
            lerp = (float)(t / _time);

            if(m_mainLight) m_mainLight.color = Color.Lerp(m_currentEnvironment.LightColor, _environment.LightColor, lerp);
            if(m_mainLight) m_mainLight.intensity = Mathf.Lerp(m_currentEnvironment.LightIntensity, _environment.LightIntensity, lerp);

            Shader.SetGlobalColor(m_ambientColorProperty, Color.Lerp(m_currentEnvironment.AmbientColor, _environment.AmbientColor, lerp));
            Shader.SetGlobalColor(m_cloudColorProperty, Color.Lerp(m_currentEnvironment.CloudColor, _environment.CloudColor, lerp));

            t += GameTime.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        m_currentEnvironment = _environment;
        m_isTransitioning = false;
        yield return null;
    }
}
