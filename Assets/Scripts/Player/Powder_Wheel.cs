using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Powder_Wheel : MonoBehaviour
{

    [SerializeField] private Transform m_wheelTransform;

    [SerializeField] private List<Transform> m_wheelChildren;


    private bool m_open = false;

    [SerializeField] private int m_closestSegment = 0;
    [SerializeField] private float m_mouseAngle = 0;
    private Vector2 p;


    private void Update()
    {
        if (PlayerInput.PowderWheel) OpenWheel();
        else CloseWheel();

        if (m_open == false) return;

        if (PlayerInput.PowderWheel)
        {
            p = new Vector2(Input.mousePosition.x - (Screen.width / 2.0f), Input.mousePosition.y - (Screen.height / 2.0f));
            m_closestSegment = 0;
            m_mouseAngle = (float)((Mathf.Atan2(p.x, p.y) / Mathf.PI) * 180f);
            if (m_mouseAngle < 0.0f) m_mouseAngle += 360.0f;
            float a = 360.0f / 8.0f;
            float a2 = a / 2.0f;

            for(int i = 0; i < 8; i++)
            {
                if(m_mouseAngle >= (i * a) - a2)
                {
                    m_closestSegment = i;
                }
            }

            for (int i = 0; i < 8; i++)
            {
                if (m_closestSegment == i)
                {
                    m_wheelChildren[i].localScale = new Vector3(1.25f, 1.25f, 1.0f);
                }
                else
                {
                    m_wheelChildren[i].localScale = Vector3.one;
                }
            }
        }
        else
        {
            CloseWheel();
        }
    }

    public void OpenWheel()
    {
        m_wheelTransform.gameObject.SetActive(true);
        m_open = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameTime.Pause();
    }

    private void CloseWheel()
    {
        m_wheelTransform.gameObject.SetActive(false);
        m_open = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameTime.UnPause();
    }

}
