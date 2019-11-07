using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Falltree : MonoBehaviour
{
    public float fspeed=10f;
    Vector3 fallaxis = new Vector3();
    float angle = 0f;
    bool fallen = false;
    float No_0_random(float rmin,float rmax)
    {
        float a=0;
        while (a == 0)
        {
            a = Random.Range(rmin, rmax);
        }
        return a;
    }
  public void Fall()
    {
        fallaxis.x = No_0_random(-1f, 1f);
        fallaxis.y = 0;
        fallaxis.z = No_0_random(-1f, 1f);
        fallaxis = fallaxis.normalized;
        StartCoroutine(TreeFall());
    }
    private void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Darkcloud")&&!fallen)
        {
            fallen = true;
            Fall();
        }
    }
    IEnumerator TreeFall()
    {
        while (angle < 90)
        {
            float x = Time.deltaTime * fspeed;
            this.transform.Rotate(fallaxis, x);
            angle += x;
            fspeed = fspeed * 1.02f;
            yield return new WaitForFixedUpdate();
        }
    }
}
