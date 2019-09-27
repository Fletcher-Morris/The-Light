using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Snake_Body : MonoBehaviour
{
    private List<Transform> m_bones;
    private void Start()
    {
        m_bones = new List<Transform>();
        Transform boneHolder = transform.Find("Bones");
        for(int i = 0; i < boneHolder.childCount; i++)
        {
            m_bones.Add(boneHolder.GetChild(i));
        }
        m_bones[0].GetComponent<ConfigurableJoint>().connectedBody = GetComponent<Rigidbody>();
        for(int i = 1; i < m_bones.Count; i++)
        {
            m_bones[i].GetComponent<ConfigurableJoint>().connectedBody = m_bones[i-1].GetComponent<Rigidbody>();
        }
        boneHolder.parent = null;
    }
}