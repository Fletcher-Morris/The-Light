using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeToNight : MonoBehaviour
{
    public Light sunlight;
    public Material darknight;
    public GameObject vfx1;
    bool triggered=false;
    // Start is called before the first frame update
    void Start()
    {
        enabled = false;
    }
    void Changetonight()
    {
        if (!triggered)
        {
            
            triggered = true;
            sunlight.intensity = 0.33f;
            sunlight.color = (new Color(29f / 255f, 69f / 255f, 155 / 255f));
            vfx1.SetActive(true);
            RenderSettings.ambientIntensity = 0.2f;
            RenderSettings.skybox = darknight;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Changetonight();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
