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
    private int m_lightColorProperty;
    private int m_cloudColorProperty;

    [SerializeField] private bool m_forceUpdate = false;

    [SerializeField] private List<Camera> m_cameras = new List<Camera>();

    private void Start()
    {
        Init();

        LerpToEnvironment(m_initialEnvironment, 1.0f, true);
    }

    private bool m_initialized = false;
    private void Init()
    {
        m_ambientColorProperty = Shader.PropertyToID("AmbientColor");
        m_lightColorProperty = Shader.PropertyToID("LightColor");
        m_cloudColorProperty = Shader.PropertyToID("CloudColor");
        m_mainLight = GameObject.Find("Directional Light").GetComponent<Light>();
        m_initialized = true;
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

        while(t <= _time)
        {
            lerp = (float)(t / _time);
            lerp = lerp.Clamp01();
            LerpToEnvironment(_environment, lerp, false);
            t += GameTime.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        m_currentEnvironment = _environment;
        m_isTransitioning = false;
        yield return null;
    }

    public void LerpToEnvironment(EnvironmentSettings _environment, float _lerp, bool _setCurrent)
    {
        if (m_initialized == false) Init();
        if (_environment == null) return;
        if (m_currentEnvironment == null) m_currentEnvironment = m_initialEnvironment;

        _lerp = _lerp.Clamp01();
        if (m_mainLight) m_mainLight.color = Color.Lerp(m_currentEnvironment.LightColor, _environment.LightColor, _lerp);
        if (m_mainLight) m_mainLight.intensity = Mathf.Lerp(m_currentEnvironment.LightIntensity, _environment.LightIntensity, _lerp);
        Shader.SetGlobalColor(m_ambientColorProperty, Color.Lerp(m_currentEnvironment.AmbientColor, _environment.AmbientColor, _lerp));
        Shader.SetGlobalColor(m_lightColorProperty, Color.Lerp(m_currentEnvironment.LightColor, _environment.LightColor, _lerp));
        Shader.SetGlobalColor(m_cloudColorProperty, Color.Lerp(m_currentEnvironment.CloudColor, _environment.CloudColor, _lerp));
        Color col = Color.Lerp(m_currentEnvironment.SkyColor, _environment.SkyColor, _lerp);
        foreach (Camera cam in m_cameras) { cam.backgroundColor = col; }

        if (_setCurrent) m_currentEnvironment = _environment;
    }

    public EnvironmentSettings GetCurrentEnvironment()
    {
        if (m_currentEnvironment != null) return m_currentEnvironment;
        else if (m_initialEnvironment != null) return m_initialEnvironment;
        else return null;
    }

    private void Update()
    {
        if(m_forceUpdate)
        {
            Transition(m_currentEnvironment, 0.1f);
            m_forceUpdate = false;
        }
    }
}
