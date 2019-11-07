using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanCamp_Part1 : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] AudioSource audiosource;

    //Soldiers
    [Header("Soldier")]
    [SerializeField] List<AudioClip> Soldiersclips = new List<AudioClip>();
    [SerializeField] Animator SoldierA;
    //public bool EndRead, EndParchment, EndStandUp, EndWalk = false;
    bool SoldiersAreSitting = true;

    //Chief
    [Header("Chief")]
    [SerializeField] Transform Parchment;
    [SerializeField] Transform ParchmentStatic;
    [SerializeField] AudioClip Chiefclip;
    [SerializeField] Animator ChiefAnim;
    [SerializeField] Transform ChiefEnd;

    // Start is called before the first frame update
    void Start()
    {
        //Set parchments
        Parchment.gameObject.SetActive(true);
        ParchmentStatic.gameObject.SetActive(false);

        StartCoroutine(SoldierTalk());
        StartCoroutine(ChiefComes());
    }

    int random, previous = 0;
    IEnumerator SoldierSitting()
    {
        while (SoldiersAreSitting)
        {
            //Random animation IDLE for soldier
            random = Random.Range(1, 4); //4 is the number of animation available -1 (as 4 is exluded)
            while (random == previous)
                random = Random.Range(1, 4); // Avoid having same animation several time
            SoldierA.SetBool("IDLE_talking" + random.ToString(), true);
            SoldierA.SetBool("IDLE_talking" + previous.ToString(), false);
            previous = random;
            yield return new WaitForSeconds(Random.Range(25f,50f));
        }
       
    }

    //Part 1 A: Soldier talks while the chief waits
    IEnumerator SoldierTalk()
    {
        StartCoroutine(SoldierSitting());
        foreach (AudioClip clip in Soldiersclips)
        {
            //Wait for the previous audio to finish
            while (audiosource.isPlaying)
                yield return null;

            audiosource.clip = clip;
            audiosource.Play();
        }
    }

    float speed = 1.5f;
    //Part 1 B: Chief put the parchment on the table and go to the soldier
    IEnumerator ChiefComes() {

        //for the soldiers to talk a bit
        yield return new WaitForSeconds(1f); //see to change according to what people says

        //chief puts the parchment done
        ChiefAnim.SetBool("ParchmentDown", true);

        //Wait for the animation to finish
        yield return new WaitUntil(() => ChiefAnim.GetCurrentAnimatorStateInfo(0).IsName("ChiefParchment") && ChiefAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);


        //Move chief to soldiers
        ChiefAnim.SetBool("WalkToSoldier", true);

        //remove the parchment from the player's hand (change parent)
        Parchment.gameObject.SetActive(false);
        //Destroy(Parchment.gameObject);
        ParchmentStatic.gameObject.SetActive(true);

        yield return new WaitUntil(() => ChiefAnim.GetCurrentAnimatorStateInfo(0).IsName("ChiefStandUp") && ChiefAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);

        //Move the character from point A to point B
        while (Vector3.Distance(ChiefAnim.transform.position, ChiefEnd.position) >= 0.01f)
        {
            Debug.Log(Vector3.Distance(ChiefAnim.transform.position, ChiefEnd.position));
            // Move our position a step closer to the target.
            float step = speed * Time.deltaTime; // calculate distance to move
            ChiefAnim.transform.position = Vector3.MoveTowards(ChiefAnim.transform.position, ChiefEnd.position, step);
            yield return null;
        }

        StartCoroutine(chiefTalk());

    }


    //Part 1 C: Chief talks to the soldiers
    IEnumerator chiefTalk()
    {
        //chief talks
        ChiefAnim.SetBool("Talking", true);

        audiosource.clip = Chiefclip;
        audiosource.Play();
        //Wait for the previous audio to finish
        while (audiosource.isPlaying)
                yield return null;
        //To add: call part 2 animation
    }
}
