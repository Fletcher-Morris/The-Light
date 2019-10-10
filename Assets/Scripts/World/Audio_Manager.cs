using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio_Manager : MonoBehaviour
{
    private static Audio_Manager m_singleton;
    public static Audio_Manager Singleton() { return m_singleton; }

    private void Awake()
    {
        m_singleton = this;

        m_maxFootstepId = playerFootstepsDirt.Count;
        if (playerFootstepsGrass.Count < m_maxFootstepId) m_maxFootstepId = playerFootstepsGrass.Count;
        if (playerFootstepsSand.Count < m_maxFootstepId) m_maxFootstepId = playerFootstepsSand.Count;
        if (playerFootstepsStone.Count < m_maxFootstepId) m_maxFootstepId = playerFootstepsStone.Count;
        if (playerFootstepsWood.Count < m_maxFootstepId) m_maxFootstepId = playerFootstepsWood.Count;
        if (playerFootstepsWater.Count < m_maxFootstepId) m_maxFootstepId = playerFootstepsWater.Count;
        m_maxFootstepId--;
    }


    [Header("Footsteps")]
    public List<AudioClip> playerFootstepsDirt;
    public List<AudioClip> playerFootstepsGrass;
    public List<AudioClip> playerFootstepsSand;
    public List<AudioClip> playerFootstepsStone;
    public List<AudioClip> playerFootstepsWood;
    public List<AudioClip> playerFootstepsWater;
    private int m_maxFootstepId = 0;
    public int MaxFootstepId() { return m_maxFootstepId; }
    public AudioClip GetFootstepAudio(GroundAudioType _type, int _index)
    {
        AudioClip clip = null;

        if(_index == -1)
        {
            _index = Random.Range(0, m_maxFootstepId);
        }

        switch (_type)
        {
            case GroundAudioType.dirt:
                return playerFootstepsDirt[_index];
            case GroundAudioType.grass:
                return playerFootstepsGrass[_index];
            case GroundAudioType.sand:
                return playerFootstepsSand[_index];
            case GroundAudioType.stone:
                return playerFootstepsStone[_index];
            case GroundAudioType.wood:
                return playerFootstepsWood[_index];
            case GroundAudioType.water:
                return playerFootstepsWater[_index];
        }

        return clip;
    }
}
