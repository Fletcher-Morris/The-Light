using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground_Audio : MonoBehaviour
{
    [SerializeField] GroundAudioType m_audioType = GroundAudioType.dirt;
    public GroundAudioType GetGroundType() { return m_audioType; }
}

public enum GroundAudioType
{
    dirt,
    grass,
    sand,
    stone,
    wood,
    water
}