using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;



    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        StartCalm();
    }

    [SerializeField] AudioSource[] calm;
    [SerializeField] AudioSource[] panic;

    public void StartCalm()
    {
        foreach (AudioSource audio in calm)
            audio.enabled = true;
        foreach (AudioSource audio in panic)
            audio.enabled = false;
    }

    public void StartHelp()
    {
        foreach (AudioSource audio in calm)
            audio.enabled = false;
        foreach (AudioSource audio in panic)
            audio.enabled = true;
    }
}
