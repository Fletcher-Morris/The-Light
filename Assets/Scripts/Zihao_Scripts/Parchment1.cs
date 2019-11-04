using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parchment1 : MonoBehaviour
{

    [SerializeField] private GameObject pa;
    private Material paper;
    Animator anim;
    void Start()
    {
        paper = pa.GetComponent<MeshRenderer>().material;
        //Close the parchment to frame 1
        anim = this.gameObject.GetComponent<Animator>();
        anim.speed = 0f;
        anim.Play("ParchmentAnim");
       
    }
  public void OnTriggerStay(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            if (GameProcessManager.instance.ReadCondition("collectparchment") != 1)
            {
               
                if (Input.GetKeyDown(KeyCode.E))
                {
                    GameProcessManager.instance.SetCondition("collectparchment", 1);
                    MoveToCamera();
                }
            }
        }
    }
    
   
    public void MoveToCamera()
    {
        //Get the camera
        Transform cam = Camera.main.transform;

        //Become child of the camera
        transform.parent = cam;

        //Move parchment in front of the camera
        transform.localPosition = new Vector3(0, 0, 0.463f);
        //Rotate the parchment to be in the right way
        transform.localScale = new Vector3(2.031203f, 2.031203f, 2.031203f);
        transform.localRotation = Quaternion.Euler(0, -90f, 90f);


        //Animate the parchment to open up
        anim.speed = 1f;
        anim.Play("ParchmentAnim");

        //Play the Audio
        GetComponent<AudioSource>().Play();
        StartCoroutine(openparchment());
        StartCoroutine(CheckKey());
    }
    IEnumerator openparchment()
    {
        float x = 1;
        float t=60;
        for(float i = 0; i < t; i++)
        {
            x = x - 1/t;
            paper.SetFloat("_V1", x);
            yield return new WaitForSeconds(0.015f);
        }
        
    }
    IEnumerator CheckKey()
    {
        //Check when Escape is pressed to delete the parchment
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameProcessManager.instance.SetCondition("parchment", 1);
                Destroy(this.gameObject);

            }
            yield return null;
        }
    }
}
