using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectLamp : MonoBehaviour
{
    public GameObject a1, a2;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            a1.SetActive(true);
            a2.SetActive(true);
            GameProcessManager.instance.SetCondition("lamp", 1);
            Destroy(this.gameObject);
        }
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
