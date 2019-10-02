using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SetAmbientColor : MonoBehaviour
{

    [SerializeField] private Vector4 m_ambientColor = Vector4.one;
    private Vector4 m_prevColor = Vector4.one;

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
