using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanCamp_Part2 : MonoBehaviour
{
    [Header("Patroller")]
    [SerializeField] Animator PatrollerAnim, BucketAnim;
    [SerializeField] Transform PointB, PointC, PointD;

    [Header("Soldier And Chief")]
    [SerializeField] Transform SoldierA_Exit;
    [SerializeField] Transform  SoldierB_Exit;
    [SerializeField] Transform Chief_Exit;
    Animator SoldierA, SoldierB, Chief;
    float speed = 5f;

    AudioSource audiosource;
    [SerializeField] AudioClip PatrolClip1, PatrolClip2;
    HumanCamp_Part1 part1;

    // Start is called before the first frame update
    void Start()
    {
        PatrollerAnim.gameObject.SetActive(false);

        part1 = GetComponent<HumanCamp_Part1>();
        SoldierA = part1.GetSoldierA();
        SoldierB = part1.GetSoldierB();
        Chief = part1.GetChief();

        audiosource = GetComponent<AudioSource>();
        BucketAnim.enabled = false;
        //StartCoroutine(PatrollerActions());
    }

    public void StartAction()
    {
        StartCoroutine(PatrollerActions());
    }

    IEnumerator PatrollerActions()
    {
        PatrollerAnim.gameObject.SetActive(true);
        audiosource.clip = PatrolClip1;
        audiosource.Play();
        //Run from point A to B
        //Move the character from point A to point B
        while (Vector3.Distance(PatrollerAnim.transform.position, PointB.position) >= 0.01f)
        {
            // Move our position a step closer to the target.
            float step = speed * Time.deltaTime; // calculate distance to move
            PatrollerAnim.transform.position = Vector3.MoveTowards(PatrollerAnim.transform.position, PointB.position, step);
            PatrollerAnim.transform.LookAt(PointB);
            yield return null;
        }

        //Trip on the bucket C
        PatrollerAnim.SetBool("Falling" , true);
        BucketAnim.enabled = true; //Animation of the bucket
        //Make the solier react
        SoldierA.SetBool("LookTowardPatroller", true);
        SoldierB.SetBool("LookTowardPatroller", true);
        Chief.SetBool("LookTowardPatroller", true);
        while (Vector3.Distance(PatrollerAnim.transform.position, PointC.position) >= 0.01f) //MoveFall
        {
            // Move our position a step closer to the target.
            float step = speed * Time.deltaTime; // calculate distance to move
            PatrollerAnim.transform.position = Vector3.MoveTowards(PatrollerAnim.transform.position, PointC.position, step);
            PatrollerAnim.transform.LookAt(PointC);
            yield return null;
        }
        yield return new WaitUntil(() => PatrollerAnim.GetCurrentAnimatorStateInfo(0).IsName("PatrollerFall") && PatrollerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
        PatrollerAnim.SetBool("Falling", false);


        //Get back up (look at the corruption and point it?)
        yield return new WaitUntil(() => PatrollerAnim.GetCurrentAnimatorStateInfo(0).IsName("PatrollerShowCorruption"));
        //PlaySecond "Run!"
        audiosource.clip = PatrolClip2;
        audiosource.Play();

        //Soldiers and Chief look at there
        SoldierA.SetBool("LookAtCorruption", true);
        SoldierB.SetBool("LookAtCorruption", true);
        Chief.SetBool("LookAtCorruption", true);

        yield return new WaitUntil(() => PatrollerAnim.GetCurrentAnimatorStateInfo(0).IsName("PatrollerRun"));
        StartCoroutine(SoldierRuns());
        //run from point C to D
        while (Vector3.Distance(PatrollerAnim.transform.position, PointD.position) >= 0.01f) //MoveFall
        {
            // Move our position a step closer to the target.
            float step = speed * Time.deltaTime; // calculate distance to move
            PatrollerAnim.transform.position = Vector3.MoveTowards(PatrollerAnim.transform.position, PointD.position, step);
            PatrollerAnim.transform.LookAt(PointD);
            yield return null;
        }
        PatrollerAnim.SetBool("Dissapear", true);
    }

    IEnumerator SoldierRuns()
    {
        GetComponent<HumanCamp_Part3>().StartPart3();
        yield return new WaitForSeconds(1f);

        SoldierB.SetBool("DoRun", true);
        SoldierA.SetBool("DoRun", true);
        Chief.SetBool("DoRun", true);

        while (Vector3.Distance(SoldierA.transform.position, SoldierA_Exit.position) >= 0.01f ||
           Vector3.Distance(SoldierB.transform.position, SoldierB_Exit.position) >= 0.01f ||
           Vector3.Distance(Chief.transform.position, Chief_Exit.position) >= 0.01f) 
        {
            // Move our position a step closer to the target.
            float step = speed * Time.deltaTime; // calculate distance to move
            SoldierA.transform.position = Vector3.MoveTowards(SoldierA.transform.position, SoldierA_Exit.position, step);
            SoldierA.transform.LookAt(SoldierA_Exit);

            SoldierB.transform.position = Vector3.MoveTowards(SoldierB.transform.position, SoldierB_Exit.position, step);
            SoldierB.transform.LookAt(SoldierB_Exit);

            Chief.transform.position = Vector3.MoveTowards(Chief.transform.position, Chief_Exit.position, step);
            Chief.transform.LookAt(Chief_Exit);

            yield return null;
        }

        SoldierB.SetBool("Dissapear", true);
        SoldierA.SetBool("Dissapear", true);
        Chief.SetBool("Dissapear", true);
    }
}
