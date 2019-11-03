using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corruption : MonoBehaviour
{
    Animator anim;
    public Animator Treeanim;
    //[SerializeField] Transform Camera;
    //public float Distance;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
        anim.enabled = false;
        Treeanim.enabled = false;
    }

    // Update is called once per frame
    public void PlayCorruption()
    {
        foreach (ParticleSystem ps in GetComponentsInChildren< ParticleSystem>())
            ps.Play();

        anim.enabled = true;
        anim.speed = 0.05f;
        anim.Play("CorruptionCloud");

        Treeanim.enabled = true;
        Treeanim.speed = 0.05f;
        Treeanim.Play("TreesFalling");
    }

}
