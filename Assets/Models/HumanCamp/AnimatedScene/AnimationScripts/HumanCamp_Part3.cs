using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanCamp_Part3 : MonoBehaviour
{
    [SerializeField] Transform Corruption;
    [SerializeField] Transform[] CorruptionPath;
    float speed = 5f;

    private void Start()
    {
        Corruption.gameObject.SetActive(false);
    }

    // Update is called once per frame
    public void StartPart3()
    {
        Corruption.gameObject.SetActive(true);
        StartCoroutine(CorruptionInvasion());
    }

    IEnumerator CorruptionInvasion()
    {
        
        //Corruption goes into the camp
        foreach (Transform point in CorruptionPath)
        {
            while (Vector3.Distance(Corruption.transform.position, point.position) >= 0.01f)
            {
                // Move our position a step closer to the target.
                float step = speed * Time.deltaTime; // calculate distance to move
                Corruption.transform.position = Vector3.MoveTowards(Corruption.transform.position, point.position, step);
                Corruption.transform.rotation = Quaternion.Slerp(Corruption.transform.rotation, Quaternion.LookRotation(point.position - Corruption.transform.position), step);
                yield return null;
            }
        }
        Corruption.GetComponent<Animator>().SetBool("Dissapear", true);
        yield return null;
    }
}
