using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class delemt : MonoBehaviour
{
    public int c = 3;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (c <= 0)
            Destroy(this.gameObject);
    }
}
