using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.VFX;
public class Darkcloud : MonoBehaviour
{
    float movespeed = 3f;
    Vector3 vector_PlayerToThis = new Vector3(1, 0, 0);
    Vector3 vector_movedirection = new Vector3(1, 0, 0);
    float distance, ct = 0f;
    bool inlight, lastframe_inlight = false;
    bool move = false;
    float rotateangle;
    // Start is called before the first frame update
    void Start()
    {
        rotateangle = Random.Range(-0.4f, 0.4f);
    }

    void SetMoveDirection()
    {
        //normalized vector from player to this
        vector_movedirection.x = vector_PlayerToThis.x;
        vector_movedirection.y = 0f;
        vector_movedirection.z = vector_PlayerToThis.z;

        vector_movedirection = vector_movedirection.normalized * movespeed;
        //rotate to make it more random
        vector_movedirection.x = vector_movedirection.x * Mathf.Cos(rotateangle) + vector_movedirection.z * Mathf.Sin(rotateangle);
        vector_movedirection.z = -vector_movedirection.x * Mathf.Sin(rotateangle) + vector_movedirection.z * Mathf.Cos(rotateangle);

    }
    // Update is called once per frame
    void Update()
    {
        if (Player_Controller.Singleton() == null) return;


        vector_PlayerToThis = this.gameObject.transform.position - Player_Controller.Singleton().transform.position;
        distance = Mathf.Sqrt(vector_PlayerToThis.x * vector_PlayerToThis.x + vector_PlayerToThis.y * vector_PlayerToThis.y + vector_PlayerToThis.z * vector_PlayerToThis.z);
        lastframe_inlight = inlight;

        if (distance < 30f)
        {
            if (Inventory_Controller.Singleton().HasItemInInventory("The Lamp"))
            {
                inlight = true;
                if (lastframe_inlight != inlight)
                {
                    this.gameObject.GetComponent<VisualEffect>().SendEvent("InLight");
                }
                if (distance < 10)
                {
                    move = true;


                }

            }
        }
        else
        {
            inlight = false;

            if (lastframe_inlight != inlight)
            {
                this.gameObject.GetComponent<VisualEffect>().SendEvent("OutLight");
            }
        }

        if (move)
        {
            if (ct == 0f)
            {
                this.gameObject.GetComponent<VisualEffect>().SendEvent("Fade");
            }
            ct = ct + GameTime.deltaTime;
            this.gameObject.GetComponent<VisualEffect>().SetFloat("x", vector_movedirection.normalized.x);
            this.gameObject.GetComponent<VisualEffect>().SetFloat("z", vector_movedirection.normalized.z);
            if (ct > 2f)
            {
                Destroy(this.gameObject);
            }
            SetMoveDirection();
            this.transform.position = new Vector3(this.transform.position.x + vector_movedirection.x * GameTime.deltaTime, this.transform.position.y, this.transform.position.z + vector_movedirection.z * GameTime.deltaTime);

        }
    }
}
