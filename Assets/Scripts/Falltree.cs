using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Falltree : MonoBehaviour
{
    [SerializeField] private float fspeed=10f;
    [SerializeField] Vector3 fallaxis = new Vector3();
    [SerializeField] bool RandomFall=true;
    float angle = 0f;
    bool fallen = false;

    private AudioSource m_audio;
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
      //Random axis if the axis is vector(0,0,0)||choose Randomfall
        if((fallaxis.x*fallaxis.x+fallaxis.y*fallaxis.y+fallaxis.z*fallaxis.z)==0f||RandomFall)
        {
            fallaxis.x = No_0_random(-1f, 1f);
            fallaxis.y = 0;
            fallaxis.z = No_0_random(-1f, 1f);
        }
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
        m_audio = GetComponent<AudioSource>();
        m_audio.Play();
        Debug.Log("Tree Is Falling! : " + gameObject);
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
