using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class Build_Version : MonoBehaviour
{
    [SerializeField] private string m_prefix = "v ";
    private string m_prevPrefix;
    private void Update()
    {
        if (m_prefix != m_prevPrefix) Refresh();
    }
    private void OnEnable()
    {
        Refresh();
    }

    private void Refresh()
    {
        Text text = GetComponent<Text>();
        if (text != null)
        {
            text.text = m_prefix + Application.version.ToString();
        }
        m_prevPrefix = m_prefix;
    }
}
