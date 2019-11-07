using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public enum R_axis
    {
        x ,
        y ,
        z
    }
    public bool clockwise;
    public float speed;
    public R_axis axis;
    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindGameObjectWithTag("Watermill").GetComponent<Watermill>().AddRotator(this);
    }

   
}
