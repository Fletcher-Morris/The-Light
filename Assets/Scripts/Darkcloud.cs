using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Darkcloud : MonoBehaviour
{
   [SerializeField] GameObject player;
    Vector3 distance=new Vector3();
    float d;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
        distance = this.gameObject.transform.position - player.transform.position;
        d = Mathf.Sqrt(distance.x * distance.x + distance.y * distance.y + distance.z * distance.z);
        if (d < 10f)
        {
            if (player.GetComponent<Inventory_Controller>().HasItemInInventory("The Lamp"))
            {
                Destroy(this.gameObject);
            }
        }
    }
}
