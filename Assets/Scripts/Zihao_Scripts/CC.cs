using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CC : MonoBehaviour
{
    GameObject player;
    float speed;
    Vector3 dir;
    // Start is called before the first frame update
    void Start()
    {
        speed = 4f;
        player = GameObject.FindGameObjectWithTag("Player");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LightRange")){
            GameProcessManager.instance.ChangeCondition("cloudkillcount", 1);
            Destroy(this.gameObject);
        }
    }
    // Update is called once per frame
    void Update()
    {
        dir = -1*(this.transform.position - player.transform.position).normalized;
        this.transform.position += dir * speed*Time.deltaTime;
    }
}
