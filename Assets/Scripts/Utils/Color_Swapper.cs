using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Color_Swapper : MonoBehaviour
{
    [SerializeField] private Color m_colorA = Color.red;
    [SerializeField] private Color m_colorB = Color.blue;

    private int i = 0;

    public void Swap()
    {
        if (i == 0)
        {
            GetComponent<MeshRenderer>().material.SetColor("_Color", m_colorA);
            i = 1;
        }
        else
        {
            GetComponent<MeshRenderer>().material.SetColor("_Color", m_colorB);
            i = 0;
        }
    }
}
