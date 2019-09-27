using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Snake_Body : MonoBehaviour
{
    [SerializeField] private float m_headLerp = 0.1f;
    [SerializeField] private float m_headHeight = 2.5f;
    [SerializeField] private Vector3 m_headSway;
    [SerializeField] private float m_swaySpeed = 5.0f;

    private List<Transform> m_bones;

    private Enemy_Ai m_ai;
    private void Start()
    {
        m_ai = GetComponent<Enemy_Ai>();
        m_bones = new List<Transform>();
        Transform boneHolder = transform.Find("Bones");
        for(int i = 0; i < boneHolder.childCount; i++)
        {
            m_bones.Add(boneHolder.GetChild(i));
        }
        //m_bones[0].GetComponent<ConfigurableJoint>().connectedBody = GetComponent<Rigidbody>();
        for(int i = 1; i < m_bones.Count; i++)
        {
            m_bones[i].GetComponent<ConfigurableJoint>().connectedBody = m_bones[i-1].GetComponent<Rigidbody>();
        }
        boneHolder.parent = null;
    }

    private void LateUpdate()
    {
        Transform head = m_bones[0];

        Vector3 targetPos;
        switch (m_ai.GetAiState())
        {
            case Ai_State.Idle:
                targetPos = transform.position + new Vector3(0.0f, m_headHeight, 0.0f);
                targetPos += head.right * m_headSway.x * Mathf.Sin(Time.time * m_swaySpeed);
                head.position = Vector3.Lerp(head.position, targetPos, m_headLerp * Time.deltaTime);
                head.rotation = Quaternion.Lerp(head.rotation, transform.rotation, 0.5f);
                break;
            default:
                targetPos = transform.position + new Vector3(0.0f, m_headHeight / 2.0f, 0.0f);
                targetPos += head.right * m_headSway.x * 5 * Mathf.Sin(Time.time * m_ai.GetSettings().chaseMoveSpeed);
                head.position = Vector3.Lerp(head.position, targetPos, m_headLerp * Time.deltaTime);
                head.rotation = Quaternion.Lerp(head.rotation, transform.rotation, 0.5f);
                break;
        }

    }
}