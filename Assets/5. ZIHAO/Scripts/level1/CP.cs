using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CP : MonoBehaviour
{
    public GameObject CCL;
    GameObject a;
    float cd;
    Vector3 t;
    // Start is called before the first frame update
    void Start()
    {
        cd = 1f;
      t = this.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        cd = cd - Time.deltaTime;
        if (cd < 0f)
        {
            cd = 4f;
            a=Instantiate(CCL);
            a.transform.position = t;
        }
    }
}
