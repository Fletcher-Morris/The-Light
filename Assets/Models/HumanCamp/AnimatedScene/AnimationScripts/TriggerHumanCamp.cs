using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerHumanCamp : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Transform campCharacters;
    [SerializeField] HumanCamp_Part1 camp;

    private void Awake()
    {
        campCharacters.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //StartCoroutine(camp.Part1());
            campCharacters.gameObject.SetActive(true);
            camp.DoStart();
        }
    }
}
