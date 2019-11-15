using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyafterx : MonoBehaviour
{
   float cd = 2;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void FixedUpdate()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        cd = cd - GameTime.deltaTime;
        if (cd < 0)
        {
            Destroy(this.gameObject);
        }
    }
}
