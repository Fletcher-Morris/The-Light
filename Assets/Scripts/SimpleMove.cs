using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMove : MonoBehaviour
{
    public GameObject target;
    Vector3 vt = new Vector3();
    public float mspeed;
    // Start is called before the first frame update
    void Start()
    {
        vt = target.transform.position - this.gameObject.transform.position;
        vt = vt.normalized;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += vt * mspeed * Time.deltaTime;
    }
}
