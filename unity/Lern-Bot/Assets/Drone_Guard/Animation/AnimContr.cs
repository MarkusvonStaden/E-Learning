using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimContr : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator anim;

    void Start()
    {
        anim = GetComponent<Animator>();

        anim.speed = 2;
        anim.Play("WakeUp");       
    }
}
