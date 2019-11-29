using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Environment", menuName = "Environment", order = 1)]
[System.Serializable]
public class EnvironmentSettings : ScriptableObject
{
    [SerializeField] private Color lightColor = Color.white;
    public Color LightColor { get => lightColor; set => lightColor = value; }

    [SerializeField] private float m_lightIntensity = 2.0f;
    public float LightIntensity { get => m_lightIntensity; set => m_lightIntensity = value; }

    [SerializeField] private Color skyColor = Color.white;
    public Color SkyColor { get => skyColor; set => skyColor = value; }

    [SerializeField] private Color cloudColor = Color.white;
    public Color CloudColor { get => cloudColor; set => cloudColor = value; }

    [SerializeField] private Color ambientColor = Color.white;
    public Color AmbientColor { get => ambientColor; set => ambientColor = value; }

    [SerializeField] private bool m_rain = false;
    public bool Rain { get => m_rain; set => m_rain = value; }
}
