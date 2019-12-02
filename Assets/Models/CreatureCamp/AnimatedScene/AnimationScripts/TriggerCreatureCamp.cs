using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCreatureCamp : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Transform campCharacters;
    [SerializeField] CreatureCamp camp;

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
