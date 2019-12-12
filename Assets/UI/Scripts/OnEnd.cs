using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OnEnd : MonoBehaviour
{

    public GameObject UIEnd;

    private void Start()
    {
        UIEnd.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player_Controller>().enabled = false;

            UIEnd.SetActive(true);
            StartCoroutine(WaitToGoToMainMenu());
        }
    }

    IEnumerator WaitToGoToMainMenu()
    {
        yield return new WaitForSeconds(3f);
        SceneManager.LoadScene(0);
    }
}
