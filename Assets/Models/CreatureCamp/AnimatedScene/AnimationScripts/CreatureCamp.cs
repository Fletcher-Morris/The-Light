using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureCamp : MonoBehaviour
{

    [Header("Audio")]
    AudioSource audiosource;
    [SerializeField] AudioClip ChildA_1;
    [SerializeField] AudioClip CreatureA_1;
    [SerializeField] AudioClip[] Dialogues;
    Animator[] DialoguesSpeaker;

    [Header("Animators")]
    [SerializeField] Animator ChildA;
    [SerializeField] Animator ChildB;
    [SerializeField] Animator ChildC;
    [SerializeField] Animator CreatureA, CreatureB, CreatureC, CreatureD;
    [SerializeField] Animator CageA, CageB, CageC;
    [SerializeField] Transform Corruption;

    [Header("Moves")]
    public Transform JumpCreatureA, JumpCreatureB, JumpCreatureC, JumpCreatureD;
    public Transform WalkACageA, WalkACageB, WalkACageC;
    public Transform WalkChildA, WalkChildB, WalkChildC;
    public Transform WalkCreatureA, WalkCreatureB, WalkCreatureC, WalkCreatureD;
    public Transform AToTent, AShowTable;
    //Escape corruption
    public Transform CorruptionPoints;
    public Transform RunChildA, RunChildB, RunChildC;
    public Transform RunCreatureA, RunCreatureB, RunCreatureC, RunCreatureD;

    Transform[] JumpCreatureAPath, JumpCreatureBPath, JumpCreatureCPath, JumpCreatureDPath;
    Transform[] WalkAPathCageA, WalkAPathCageB, WalkAPathCageC, WalkChildAPath, WalkChildBPath, WalkChildCPath;
    Transform[] WalkCreatureAPath, WalkCreatureBPath, WalkCreatureCPath, WalkCreatureDPath;
    Transform[] AToTentPath, AShowTablePath;
    Transform[] CorruptionPath;
    Transform[] RunPathChildA, RunPathChildB, RunPathChildC, RunPathCreatureA, RunPathCreatureB, RunPathCreatureC, RunPathCreatureD;

    [SerializeField] float CageBottomY;

    float speed = 1.5f;
    float speedRun = 3f;

    public void DoStart()
    {//Called from TriggerCreatureCamp
        GetAllPaths();
        Corruption.gameObject.SetActive(false);
        DialoguesSpeaker = new Animator[] { CreatureA, CreatureB, CreatureA, CreatureC, CreatureA, CreatureD };
        audiosource = GetComponent<AudioSource>();
        CageA.enabled = false;
        CageB.enabled = false;
        CageC.enabled = false;

        StartCoroutine(Part1());
    }

    Vector3 MovePosition(Vector3 Current, Vector3 Target, float step)
    {
        return Vector3.MoveTowards(Current, Target, step);
    }
    Quaternion MoveRotation(Quaternion Current, Quaternion Target, float step)
    {
        return Quaternion.RotateTowards(Current, Target, step);
    }

    IEnumerator Part1()
    {
        //the childA wakes up+ audio
        ChildA.SetBool("Cry", true);
        audiosource.clip = ChildA_1;
        audiosource.Play();

        //The CreatureA jump out of the tent after a small wait
        yield return new WaitForSeconds(ChildA_1.length - (ChildA_1.length*0.3f));

        //-START jumping out of tent - A
        //PreJump
        CreatureA.SetBool("ToJump", true);
        yield return new WaitUntil(() => CreatureA.GetCurrentAnimatorStateInfo(0).IsName("CreaturePreJump") && CreatureA.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f ); //Wait wake up

        //Jumping
        CreatureA.SetBool("Jumping", true);
        foreach (Transform point in JumpCreatureAPath)
        {
            while (Vector3.Distance(CreatureA.transform.position, point.position) >= 0.01f)
            {
                // Move our position a step closer to the target.
                float step = 12f * Time.deltaTime; // calculate distance to move
                CreatureA.transform.position = MovePosition(CreatureA.transform.position, point.position, step);
                yield return null;
            }
        }
        //Jump to stand
        CreatureA.SetBool("JumpToUp", true);
        yield return new WaitUntil(() => CreatureA.GetCurrentAnimatorStateInfo(0).IsName("CreatureJumpToUp") && CreatureA.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f); //Wait wake up
        //-END jumping out of tent - A

        //-START Go to open Cage A
        //CreatureA walk to the cage
        CreatureA.SetBool("Walk", true);
        foreach (Transform point in WalkAPathCageA)
        {
            while (Vector3.Distance(CreatureA.transform.position, point.position) >= 0.01f)
            {
                // Move our position a step closer to the target.
                float step = speed * Time.deltaTime; // calculate distance to move
                CreatureA.transform.position = MovePosition(CreatureA.transform.position, point.position, step);
                CreatureA.transform.rotation = MoveRotation(CreatureA.transform.rotation, point.rotation, step);

                yield return null;
            }
        }
        CreatureA.SetBool("Walk", false);
        CreatureA.SetBool("CageDown", true);
        //Wait for the creature to remove the rope 
        yield return new WaitUntil(() => CreatureA.GetCurrentAnimatorStateInfo(0).IsName("CreatureRemoveRope") && CreatureA.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f);

        //Make the child transform follow the cage transform
        Vector3 offset = ChildA.transform.position - CageA.transform.position;
        while (CageA.transform.localPosition.y > CageBottomY)
        {
            CageA.transform.localPosition = new Vector3(CageA.transform.localPosition.x, CageA.transform.localPosition.y -0.1f, CageA.transform.localPosition.z);
            ChildA.transform.position = CageA.transform.position + offset;
            yield return null;
        }

        //childA stands up
        ChildA.SetBool("StandUp", true);
        CreatureA.SetBool("CageDown", false);
        CreatureA.SetBool("Walk", true);
        foreach (Transform point in AShowTable)
        {
            while (Vector3.Distance(CreatureA.transform.position, point.position) >= 0.01f)
            {
                // Move our position a step closer to the target.
                float step = speed * Time.deltaTime; // calculate distance to move
                CreatureA.transform.position = MovePosition(CreatureA.transform.position, point.position, step);
                CreatureA.transform.rotation = MoveRotation(CreatureA.transform.rotation, point.rotation, step);
                yield return null;
            }
        }
        CreatureA.SetBool("Walk", false);
        CreatureA.SetBool("ShowTable", true);
        yield return new WaitUntil(() => ChildA.GetCurrentAnimatorStateInfo(0).IsName("ChildStandUp") && ChildA.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f);

        //childA Open the door
        ChildA.SetBool("Walk", true);
        while (Vector3.Distance(ChildA.transform.position, WalkChildAPath[0].position) >= 0.01f) //Move to door
        {
            // Move our position a step closer to the target.
            float step = speed * Time.deltaTime; // calculate distance to move
            ChildA.transform.position = MovePosition(ChildA.transform.position, WalkChildAPath[0].position, step);
            ChildA.transform.rotation = MoveRotation(ChildA.transform.rotation, WalkChildAPath[0].rotation, step);
            yield return null;
        }
        ChildA.SetBool("Walk", false);
        ChildA.SetBool("OpenDoor", true);
        CageA.enabled = true;


        StartCoroutine(Part2());
        //CreatureA shows the table and the childA walk to the table
        yield return new WaitUntil(() => ChildA.GetCurrentAnimatorStateInfo(0).IsName("ChildOpenCage") && ChildA.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f);
        //-END Go to open Cage A

        //-Start ChildA go to sit
        ChildA.SetBool("OpenDoor", false);
        ChildA.SetBool("Walk", true);
        foreach (Transform point in WalkChildAPath)
        {
            while (Vector3.Distance(ChildA.transform.position, point.position) >= 0.01f)
            {
                // Move our position a step closer to the target.
                float step = speed * Time.deltaTime; // calculate distance to move
                ChildA.transform.position = MovePosition(ChildA.transform.position, point.position, step);
                ChildA.transform.rotation = MoveRotation(ChildA.transform.rotation, point.rotation, step);
                yield return null;
            }
        }
        ChildA.SetBool("Sit", true);       
    }

    bool StartToTalk = false;
    IEnumerator Part2()
    {
        //CreatureA go under the tent
        CreatureA.SetBool("WalkToTent", true);
        foreach (Transform point in AToTentPath)
        {
            while (Vector3.Distance(CreatureA.transform.position, point.position) >= 0.01f)
            {
                // Move our position a step closer to the target.
                float step = speed * Time.deltaTime; // calculate distance to move
                CreatureA.transform.position = MovePosition(CreatureA.transform.position, point.position, step);
                CreatureA.transform.rotation = MoveRotation(CreatureA.transform.rotation, point.rotation, step);
                yield return null;
            }
        }

        //CreatureA yell + audio
        CreatureA.SetBool("YellToTent", true);
        audiosource.clip = CreatureA_1;
        audiosource.Play();

        StartCoroutine(CreatureA_Part2());

        yield return new WaitForSeconds(1f);
        IEnumerator b = Creature_Part2(CreatureB, 'B', JumpCreatureBPath, WalkCreatureBPath);
        StartCoroutine(b);
        IEnumerator c = Creature_Part2(CreatureC, 'C', JumpCreatureCPath, WalkCreatureCPath);
        StartCoroutine(c);
        IEnumerator d = Creature_Part2(CreatureD, 'D', JumpCreatureDPath, WalkCreatureDPath);
        StartCoroutine(d);

    }

    IEnumerator CreaturesTalk()
    {//List of who talks to get who to animate

        StartCoroutine(Part3()); //Call part 3
        for (int i =0; i < Dialogues.Length; i++)
        {
            //Play the audio
            audiosource.clip = Dialogues[i];
            audiosource.Play();

            //Play the animation on the creature
            DialoguesSpeaker[i].SetBool("IDLE", false);
            DialoguesSpeaker[i].SetBool(("Talk" + (i+2)).ToString(), true);

            yield return new WaitUntil(() => !audiosource.isPlaying);
            //To IDLE
            DialoguesSpeaker[i].SetBool("IDLE", true);
        }

        yield return null;
        
        

    }

    IEnumerator CreatureA_Part2()
    {
        //Yell to wake the other creatures
        while (audiosource.isPlaying || (CreatureA.GetCurrentAnimatorStateInfo(0).IsName("CreatureYell") && CreatureA.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f))
            yield return null;
        //Creature A goes at the cage of B + remove rope + open it
        //CreatureA walk to the cage
        SetAllBoolFalse();
        CreatureA.SetBool("GoToCage", true);
        foreach (Transform point in WalkAPathCageB)
        {
            while (Vector3.Distance(CreatureA.transform.position, point.position) >= 0.01f)
            {
                // Move our position a step closer to the target.
                float step = speed * Time.deltaTime; // calculate distance to move
                CreatureA.transform.position = MovePosition(CreatureA.transform.position, point.position, step);
                CreatureA.transform.rotation = MoveRotation(CreatureA.transform.rotation, point.rotation, step);
                yield return null;
            }
        }

        CreatureA.SetBool("CageDown", true);
        CreatureA.SetBool("GoToCage", false);
        //Wait for the creature to remove the rope 
        yield return new WaitUntil(() => CreatureA.GetCurrentAnimatorStateInfo(0).IsName("CreatureRemoveRope") && CreatureA.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f);

        
        //Make the child transform follow the cage transform
        Vector3 offset = ChildB.transform.position - CageB.transform.position;
        while (CageB.transform.localPosition.y > CageBottomY)
        {
            CageB.transform.localPosition = new Vector3(CageB.transform.localPosition.x, CageB.transform.localPosition.y - 0.1f, CageB.transform.localPosition.z);
            ChildB.transform.position = CageB.transform.position + offset;
            yield return null;
        }
  
        //Make the childA get up
        StartCoroutine(Child_Part2(ChildB, CageB, WalkChildBPath));

        //Creature A goes at the cage of C + remove rope + open it
        //CreatureA walk to the cage
        SetAllBoolFalse();
        CreatureA.SetBool("GoToCage", true);
        foreach (Transform point in WalkAPathCageC)
        {
            while (Vector3.Distance(CreatureA.transform.position, point.position) >= 0.01f)
            {
                // Move our position a step closer to the target.
                float step = speed * Time.deltaTime; // calculate distance to move
                CreatureA.transform.position = MovePosition(CreatureA.transform.position, point.position, step);
                CreatureA.transform.rotation = MoveRotation(CreatureA.transform.rotation, point.rotation, step);
                yield return null;
            }
        }

        CreatureA.SetBool("CageDown", true);
        CreatureA.SetBool("GoToCage", false);
        //Wait for the creature to remove the rope 
        yield return new WaitUntil(() => CreatureA.GetCurrentAnimatorStateInfo(0).IsName("CreatureRemoveRope") && CreatureA.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f);

        
        //Make the child transform follow the cage transform
        offset = ChildC.transform.position - CageC.transform.position;
        while (CageC.transform.localPosition.y > CageBottomY)
        {
            CageC.transform.localPosition = new Vector3(CageC.transform.localPosition.x, CageC.transform.localPosition.y - 0.1f, CageC.transform.localPosition.z);
            ChildC.transform.position = CageC.transform.position + offset;
            yield return null;
        }

        //Make the child C go to table
        StartCoroutine(Child_Part2(ChildC, CageC, WalkChildCPath));

        //Creature A join the circle
        CreatureA.SetBool("GoToCircle", true);
        foreach (Transform point in WalkCreatureAPath)
        {
            while (Vector3.Distance(CreatureA.transform.position, point.position) >= 0.01f)
            {
                // Move our position a step closer to the target.
                float step = speed * Time.deltaTime; // calculate distance to move
                CreatureA.transform.position = MovePosition(CreatureA.transform.position, point.position, step);
                CreatureA.transform.rotation = MoveRotation(CreatureA.transform.rotation, point.rotation, step);
                yield return null;
            }
        }

        //All dialogues 
        StartCoroutine(CreaturesTalk());

        yield return null;
    }

    IEnumerator Creature_Part2(Animator Creature, char Letter, Transform[] JumpPath, Transform[] CirclePath)
    {
        //-START jumping out of tent - B, C and D
        //PreJump +Awake
        Creature.SetBool("ToJump", true);
        yield return new WaitUntil(() => Creature.GetCurrentAnimatorStateInfo(0).IsName("CreaturePreJump") && Creature.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f); //Wait wake up

        //Jumping
        Creature.SetBool("Jumping", true);
        foreach (Transform point in JumpPath)
        {
            while (Vector3.Distance(Creature.transform.position, point.position) >= 0.01f)
            {
                // Move our position a step closer to the target.
                float step = 12f * Time.deltaTime; // calculate distance to move
                Creature.transform.position = MovePosition(Creature.transform.position, point.position, step);
                yield return null;
            }
        }
        //Jump to stand
        Creature.SetBool("JumpToUp", true);
        yield return new WaitUntil(() => Creature.GetCurrentAnimatorStateInfo(0).IsName("CreatureJumpToUp") /*&& Creature.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f*/); 
        //-END jumping out of tent  - B, C and D

        Creature.SetBool("Walk", true);
        //Walk to the circle
        foreach (Transform point in CirclePath)
        {
            while (Vector3.Distance(Creature.transform.position, point.position) >= 0.01f)
            {
                // Move our position a step closer to the target.
                float step = speed * Time.deltaTime; // calculate distance to move
                Creature.transform.position = MovePosition(Creature.transform.position, point.position, step);
                Creature.transform.rotation = MoveRotation(Creature.transform.rotation, point.rotation, step);
                yield return null;
            }
            yield return null;
        }

        Creature.SetBool("Wait", true);

        yield return null;
    }

    IEnumerator Child_Part2(Animator Child, Animator Cage, Transform[] PathToTable)
    {
        //childA stands up
        Child.SetBool("StandUp", true);
        yield return new WaitUntil(() => Child.GetCurrentAnimatorStateInfo(0).IsName("ChildStandUp") && Child.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f);

        //childA Open the door
        Child.SetBool("Walk", true);
        while (Vector3.Distance(Child.transform.position, PathToTable[0].position) >= 0.01f) //Move to door
        {
            // Move our position a step closer to the target.
            float step = speed * Time.deltaTime; // calculate distance to move
            Child.transform.position = MovePosition(Child.transform.position, PathToTable[0].position, step);
            Child.transform.rotation = MoveRotation(Child.transform.rotation, PathToTable[0].rotation, step);
            yield return null;
        }
        Child.SetBool("Walk", false);
        Child.SetBool("OpenDoor", true);
        Cage.enabled = true;

        yield return new WaitUntil(() => Child.GetCurrentAnimatorStateInfo(0).IsName("ChildOpenCage") && Child.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f);
        //-END Go to open Cage A

        //-Start ChildA go to sit
        Child.SetBool("OpenDoor", false);
        Child.SetBool("Walk", true);
        foreach (Transform point in PathToTable)
        {
            while (Vector3.Distance(Child.transform.position, point.position) >= 0.01f)
            {
                // Move our position a step closer to the target.
                float step = speed * Time.deltaTime; // calculate distance to move
                Child.transform.position = MovePosition(Child.transform.position, point.position, step);
                Child.transform.rotation = MoveRotation(Child.transform.rotation, point.rotation, step);
                yield return null;
            }
        }
        Child.SetBool("Sit", true);
    }

    IEnumerator Part3()
    {
        yield return new WaitForSeconds(5f);
        //Corruption appear
        Corruption.gameObject.SetActive(true);

        //Children go to the creatures
        StartCoroutine(RunWay(ChildA, RunPathChildA, 2f));
        StartCoroutine(RunWay(ChildB, RunPathChildB, 2f));
        StartCoroutine(RunWay(ChildC, RunPathChildC, 2f));

        //Everyone runs away
        StartCoroutine(RunWay(CreatureA, RunPathCreatureA, 5f));
        StartCoroutine(RunWay(CreatureB, RunPathCreatureB, 5f));
        StartCoroutine(RunWay(CreatureC, RunPathCreatureC, 5f));
        StartCoroutine(RunWay(CreatureD, RunPathCreatureD, 5f));

        //Corruption move
        foreach (Transform point in CorruptionPath)
        {
            while (Vector3.Distance(Corruption.transform.position, point.position) >= 0.01f)
            {
                // Move our position a step closer to the target.
                float step = speed * Time.deltaTime; // calculate distance to move
                Corruption.transform.position = MovePosition(Corruption.transform.position, point.position, step);
                Corruption.transform.rotation = MoveRotation(Corruption.transform.rotation, point.rotation, step);
                yield return null;
            }
        }

        Corruption.GetComponent<Animator>().SetBool("Disappear", true);
    }

    IEnumerator RunWay(Animator runner, Transform[] Path, float SectoWait)
    {
        //set anim to run
        runner.SetBool("LookAtCorruption", true);

        yield return new WaitForSeconds(SectoWait);

        //set anim to run
        runner.SetBool("RunAway", true);
        //run on path
        foreach (Transform point in Path)
        {
            while (Vector3.Distance(runner.transform.position, point.position) >= 0.01f)
            {
                // Move our position a step closer to the target.
                float step = speed * Time.deltaTime; // calculate distance to move
                runner.transform.position = MovePosition(runner.transform.position, point.position, step);
                runner.transform.rotation = MoveRotation(runner.transform.rotation, point.rotation, step);
                yield return null;
            }
        }
        runner.GetComponent<Animator>().SetBool("Disappear", true);
    }

    void SetAllBoolFalse()
    {
        CreatureA.SetBool("JumpOutOfTent", false);
        CreatureA.SetBool("Walk", false);
        CreatureA.SetBool("CageDown", false);
        CreatureA.SetBool("OpenDoor", false);
        CreatureA.SetBool("ShowTable", false);
        CreatureA.SetBool("WalkToTent", false);
        CreatureA.SetBool("YellToTent", false);
        CreatureA.SetBool("GoToCage", false);
    }

    Transform[] AllChildren(Transform parent)
    {
        Transform[] temp = new Transform[parent.childCount];
        for (int i = 0; i < parent.childCount; i++)
        {
            temp[i] = parent.GetChild(i);
        }
        return temp;
    }

    void GetAllPaths()
    {
        JumpCreatureAPath = AllChildren(JumpCreatureA);
        JumpCreatureBPath = AllChildren(JumpCreatureB);
        JumpCreatureCPath = AllChildren(JumpCreatureC);
        JumpCreatureDPath = AllChildren(JumpCreatureD);
        WalkAPathCageA = AllChildren(WalkACageA); 
        WalkAPathCageB = AllChildren(WalkACageB); 
        WalkAPathCageC = AllChildren(WalkACageC); 
        WalkChildAPath = AllChildren(WalkChildA); 
        WalkChildBPath = AllChildren(WalkChildB); 
        WalkChildCPath = AllChildren(WalkChildC); 
        WalkCreatureAPath = AllChildren(WalkCreatureA); 
        WalkCreatureBPath = AllChildren(WalkCreatureB); 
        WalkCreatureCPath = AllChildren(WalkCreatureC); 
        WalkCreatureDPath = AllChildren(WalkCreatureD); 
        AToTentPath = AllChildren(AToTent); 
        CorruptionPath = AllChildren(CorruptionPoints);
        AShowTablePath = AllChildren(AShowTable);

       RunPathChildA = AllChildren(RunChildA); 
        RunPathChildB = AllChildren(RunChildB); 
        RunPathChildC = AllChildren(RunChildC); 
        RunPathCreatureA = AllChildren(RunCreatureA);
        RunPathCreatureB = AllChildren(RunCreatureB);
        RunPathCreatureC = AllChildren(RunCreatureC);
        RunPathCreatureD = AllChildren(RunCreatureD);
    }
}
