using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerA : MonoBehaviour
{
    bool walking = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        walking = false;
        if (Input.GetKey(KeyCode.W))
        {
            walking = true;
        }
        if (Input.GetKey(KeyCode.A))
        {
            walking = true;
        }

        if (Input.GetKey(KeyCode.S))
        {
            walking = true;
        }

        if (Input.GetKey(KeyCode.D))
        {
            walking = true;
        }
        if (walking)
        {
            this.GetComponent<Animator>().SetBool("walk", true);
        }
        else
        {
            this.GetComponent<Animator>().SetBool("walk", false);
        }
    }
}
