using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Load : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if(GameProcessManager.instance.ReadCondition("lvdone")==1)
            {
                GameProcessManager.instance.ClearQuestList();
                SceneManager.LoadScene(1);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
