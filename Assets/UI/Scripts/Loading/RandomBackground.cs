using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBackground : MonoBehaviour
{
    [SerializeField] List<GameObject> Images = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        //Keep only one background randomly
        int ID =  Random.Range(0, Images.Count);
        for (int i =0; i< Images.Count; i++)
        {
            Images[i].SetActive(i == ID);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
