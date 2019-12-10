using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagFollowHands : MonoBehaviour
{

    public Transform Follower, Followed;
    public Vector3 offset;

    private void OnEnable()
    {
        Follower.parent = Followed;
    }
    // Update is called once per frame
    /*void Update()
    {
        Follower.transform.position = Followed.transform.position + offset;
        Follower.transform.rotation = Followed.transform.rotation;
    }*/
}
