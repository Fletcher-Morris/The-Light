using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parchment : MonoBehaviour
{
    Animator anim;

    void Start()
    {
        //Close the parchment to frame 1
        anim = GetComponent<Animator>();
        anim.speed = 0f;
        anim.Play("ParchmentAnim");
    }

    // Update is called once per frame
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

        StartCoroutine(CheckKey());
    }

    IEnumerator CheckKey()
    {
        //Check when Escape is pressed to delete the parchment
        while (true)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Destroy(gameObject);
            }
            yield return null;
        }
    }
}
