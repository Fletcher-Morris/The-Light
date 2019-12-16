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



    [SerializeField] private AudioSource m_forrestSource;



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


    [Header("Random Sounds")]
    public float owlTimer = 15.0f, owlChance = 40.0f;
    public AudioSource owlSource;
    public float thunderTimer = 20.0f, thunderChance = 30.0f;
    public AudioSource thunderSource;
    public float leavesTimer = 10.0f, leavesChance = 50.0f;
    public AudioSource leavesSource;


    private bool m_forrestMusicEnabled = false;
    public void EnableForrestMusic()
    {
        m_forrestMusicEnabled = true;
        owlSource.enabled = true;
    }
    public void DisableForrestMusic()
    {
        m_forrestMusicEnabled = false;
        owlSource.enabled = false;
    }


    private float m_forrestMusicTimer = 1.0f;
    private void Update()
    {
        if(m_forrestMusicEnabled)
        {
            m_forrestMusicTimer -= GameTime.deltaTime;
        }
        if(m_forrestMusicTimer <= 0.0f)
        {
            m_forrestSource.Play();
            m_forrestMusicTimer = 160.0f;
        }

        if(GameTime.IsPaused() == false)
        {
            RandomAmbiance();
        }
    }


    private float m_randomNoiseTimer = 10.0f;
    private void RandomAmbiance()
    {
        //  Thunder
        thunderTimer -= GameTime.deltaTime;
        if(thunderTimer <= 0)
        {
            int random = Random.Range(0, 100);
            if(random < thunderChance)
            {
                thunderSource.Play();
                thunderTimer = 20.0f;
                owlTimer = 15.0f;
                owlSource.Stop();
            }
        }

        //  Owl
        owlTimer -= GameTime.deltaTime;
        if (owlTimer <= 0)
        {
            int random = Random.Range(0, 100);
            if (random < owlChance)
            {
                owlSource.Play();
                owlTimer = 15.0f;
            }
        }

        //  Leaves
        leavesTimer -= GameTime.deltaTime;
        if (leavesTimer <= 0)
        {
            int random = Random.Range(0, 100);
            if (random < leavesChance)
            {
                leavesSource.Play();
                leavesTimer = 15.0f;
            }
        }
    }
}
