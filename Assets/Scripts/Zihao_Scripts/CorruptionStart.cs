using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionStart : MonoBehaviour
{
    public GameObject a1, a2, a3, a4, a5;
    public GameObject sunlight;
    public Material m1;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")){
            RenderSettings.ambientIntensity = 0.1f;
            RenderSettings.skybox = m1;
            sunlight.GetComponent<Light>().intensity = 0.2f;
            a1.SetActive(true);
            a2.SetActive(true);
            a3.SetActive(true);
            a4.SetActive(true);
            a5.SetActive(true);
            Destroy(this.gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
