using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class character_movement : MonoBehaviour
{
    public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown("w"))
        {
            animator.SetBool("walking", true);
        }
        if (Input.GetKeyUp("w"))
        {
            animator.SetBool("walking", false);
        }
        if (Input.GetKeyDown("s"))
        {
            animator.SetBool("special_moving", true);
        }
        if (Input.GetKeyUp("s"))
        {
            animator.SetBool("special_moving", false);
        }
    }
}
