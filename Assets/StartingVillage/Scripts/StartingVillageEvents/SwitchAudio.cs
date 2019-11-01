using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchAudio : MonoBehaviour
{
    [SerializeField] AudioClip clip;

    // Start is called before the first frame update
    void OnEnable()
    {
        GetComponent<AudioSource>().clip = clip;
        GetComponent<AudioSource>().Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
