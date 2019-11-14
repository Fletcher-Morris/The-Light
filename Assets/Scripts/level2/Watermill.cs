using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Watermill : MonoBehaviour
{
    public GameObject Player1,wheelupplace,wheeldownplace,waterwheel,basketupplace,basketdownplace,basket,basketpowder,boxpowder;
    public InventoryItem rawpowder;
    bool firsttimemovebasket = true;
    List<Rotator> rotators = new List<Rotator>();
    bool machinerunning = false;

    IEnumerator Movewheelup()
    {
        machinerunning = false;
        Vector3 vt = new Vector3();
        vt = wheelupplace.transform.position - waterwheel.transform.position;
        vt.x = 0;
        vt.z = 0;
        for (int i=0;i<100; i++)
        {
            waterwheel.transform.position += vt / 100;
            yield return new WaitForFixedUpdate();
        }
     
        wheelstate = 0;
    }
    IEnumerator Movewheeldown()
    {
        Vector3 vt = new Vector3();
        vt = wheeldownplace.transform.position - waterwheel.transform.position;
        vt.x = 0;
        vt.z = 0;
        for (int i = 0; i < 100; i++)
        {
            waterwheel.transform.position += vt / 100;
            yield return new WaitForFixedUpdate();
        }
        wheelstate = 2;
        machinerunning = true;
    }
    IEnumerator Movebasketdown()
    {
        Vector3 vt = new Vector3();
        vt =basketdownplace.transform.position - basket.transform.position;
        vt.x = 0;
        vt.z = 0;
        for (int i = 0; i < 50; i++)
        {
            basket.transform.position += vt / 50;
            yield return new WaitForFixedUpdate();
        }
        basketstate = 2;
    }
    IEnumerator Movebasketup()
    {
        if (firsttimemovebasket)
        {
            firsttimemovebasket = false;
            basketpowder.SetActive(true);
        }
        Vector3 vt = new Vector3();
        vt = basketupplace.transform.position - basket.transform.position;
        vt.x = 0;
        vt.z = 0;
        for (int i = 0; i < 50; i++)
        {
            basket.transform.position += vt / 50;
            yield return new WaitForFixedUpdate();
        }
        basketstate = 0;
    }
    IEnumerator Producepowder()
    {
        for (int i = 0; i < 50; i++)
        {
            
            yield return new WaitForFixedUpdate();
        }

        boxpowder.SetActive(true);
    }
   
   
    //0:up 1:moving 2:down
    int wheelstate=0;
    int basketstate = 0;
    public void AddRotator(Rotator rotator)
    {
        rotators.Add(rotator);
    }

    public void MoveWatermill()
    {
        if (wheelstate == 0)
        {
            wheelstate = 1;
            StartCoroutine(Movewheeldown());

        }
        if (wheelstate == 2)
        {
            wheelstate = 1;
            StartCoroutine(Movewheelup());
        }
    }
    public void MoveBasket()
    {
        if (machinerunning)
        {
            if (basketstate == 0)
            {
                basketstate = 1;
                StartCoroutine(Movebasketdown());
            }
            if (basketstate == 2)
            {
                basketstate = 1;
                StartCoroutine(Movebasketup());
            }
        }
    }

    public void InputRawPowder()
    {
        if (machinerunning)
        {

            if (Player1.GetComponent<Inventory_Controller>().HasItemInInventory(rawpowder))
            {
                Player1.GetComponent<Inventory_Controller>().RemoveItemFromInventory(rawpowder);
                StartCoroutine(Producepowder());
            }
        }
    }
 
    void Rotaterotator()
    {
        if (machinerunning)
        {
            foreach (Rotator rotator in rotators)
            {
                if (rotator.speed != 0)
                {
                    if (rotator.clockwise)
                    {
                        if (rotator.axis == Rotator.R_axis.x)
                        {
                            rotator.gameObject.transform.Rotate(new Vector3(1f, 0f, 0f), rotator.speed * Time.deltaTime * 100);
                        }
                        else if (rotator.axis == Rotator.R_axis.y)
                        {
                            rotator.gameObject.transform.Rotate(new Vector3(0f, 1f, 0f), rotator.speed * Time.deltaTime * 100);
                        }
                        else if (rotator.axis == Rotator.R_axis.z)
                        {
                            rotator.gameObject.transform.Rotate(new Vector3(0f, 0f, 1f), rotator.speed * Time.deltaTime * 100);
                        }
                    }
                    else
                    {
                        if (rotator.axis == Rotator.R_axis.x)
                        {
                            rotator.gameObject.transform.Rotate(new Vector3(1f, 0f, 0f), rotator.speed * Time.deltaTime * -100);
                        }
                        else if (rotator.axis == Rotator.R_axis.y)
                        {
                            rotator.gameObject.transform.Rotate(new Vector3(0f, 1f, 0f), rotator.speed * Time.deltaTime * -100);
                        }
                        else if (rotator.axis == Rotator.R_axis.z)
                        {
                            rotator.gameObject.transform.Rotate(new Vector3(0f, 0f, 1f), rotator.speed * Time.deltaTime * -100);
                        }
                    }
                    
                }
            }
        }
    }
    // Update is called once per frame
    
    void Update()
    {
        Rotaterotator();
     
    }
}
