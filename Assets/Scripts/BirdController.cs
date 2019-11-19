﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    [SerializeField] GameObject CC, MC, bird;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void PlayAnimation()
    {
        MC.SetActive(false);
        CC.SetActive(true);
        Player_Controller.Singleton().InCutscene = true;
    }
    public void Backtogame()
    {
        bird.SetActive(false);
        CC.SetActive(false);
        MC.SetActive(true);
        Player_Controller.Singleton().InCutscene = false;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
