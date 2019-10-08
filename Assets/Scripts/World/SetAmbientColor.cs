using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SetAmbientColor : MonoBehaviour
{

    [SerializeField] private Color m_ambientColor = Color.white;
    private Color m_prevColor = Color.white;

    private void Awake()
    {
        SetAmbientLightColor(m_ambientColor);
    }

    public void SetAmbientLightColor(Color _col)
    {
        Shader.SetGlobalVector("AmbientColor", _col);
        m_ambientColor = _col;
        m_prevColor = _col;
    }
    private void Update()
    {
        if(m_ambientColor != m_prevColor)
        {
            SetAmbientLightColor(m_ambientColor);
        }
    }
}
