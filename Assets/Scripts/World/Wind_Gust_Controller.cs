using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind_Gust_Controller : MonoBehaviour
{
    [SerializeField] private AudioSource m_audioSauce;

    private int m_shaderPropertyId;

    [SerializeField] private float m_multiplier = 0.15f;
    [SerializeField] private float m_gust = 0f;
    [SerializeField] private float m_lerp = 1f;

    private void Awake()
    {
        m_shaderPropertyId = Shader.PropertyToID("WindGust");
        if(TryGetComponent<AudioSource>(out AudioSource _sauce) && m_audioSauce == null)
        {
            m_audioSauce = _sauce;
        }
    }

    private void Update()
    {
        if (GameTime.IsPaused()) return;

        if (m_audioSauce == null) return;
        if (m_audioSauce.clip == null) return;
        float[] spectrum = new float[64];
        AudioListener.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
        float newGust = 0;
        for(int i = 0; i < 64; i++)
        {
            newGust += spectrum[i];
        }
        newGust /= 64;


        newGust = Mathf.Log(newGust, 10.0f);
        newGust *= -1f;

        newGust *= m_multiplier;

        if (newGust >= 0.5f) newGust = 1.0f;
        else newGust = 0.0f;

        m_gust = Mathf.Lerp(m_gust, newGust, m_lerp * GameTime.deltaTime);
        Shader.SetGlobalFloat(m_shaderPropertyId, m_gust);
    }
}
