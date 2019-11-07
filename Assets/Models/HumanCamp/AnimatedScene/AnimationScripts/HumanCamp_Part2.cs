using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HumanCamp_Part2 : MonoBehaviour
{
    [Header("Patroller")]
    [SerializeField] Animator PatrollerAnim;
    [SerializeField] Transform PointB, PointC;
    float speed = 5f;

    [Header("AnimationFixer")]
    [SerializeField] Transform HipsTransform;
    Vector3 HipsPos;

    // Start is called before the first frame update
    void Start()
    {
        HipsPos = HipsTransform.localPosition;
        StartCoroutine(PatrollerActions());
    }

    IEnumerator PatrollerActions()
    {
        //Run from point A to B
        //Move the character from point A to point B
        while (Vector3.Distance(PatrollerAnim.transform.position, PointB.position) >= 0.01f)
        {
            // Move our position a step closer to the target.
            float step = speed * Time.deltaTime; // calculate distance to move
            PatrollerAnim.transform.position = Vector3.MoveTowards(PatrollerAnim.transform.position, PointB.position, step);
            yield return null;
        }

        //Trip on the bucket C
        PatrollerAnim.SetBool("Falling" , true);
        yield return new WaitUntil(() => PatrollerAnim.GetCurrentAnimatorStateInfo(0).IsName("PatrollerFall") && PatrollerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f);
        PatrollerAnim.SetBool("Falling", false);

        //Bring the character to the animation end position to have good transition
        //PatrollerAnim.transform.localPosition = PatrollerAnim.transform.localPosition - HipsPos; //the hip pos will be changed from next animation

        //Get back up (look at the corruption and point it?)

        //run from point C to D
        yield return null;
    }
}
