using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    Rigidbody rigid;

    Animator animator;

    public float JumpPower = 5f;


    void Start()
    {

        rigid = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    
    }

    void Update()
    {
        
        Jump();
        
    }

    void Jump()
    {

        if (Input.GetKeyDown(KeyCode.F))
        {

            rigid.AddForce(Vector3.up * JumpPower, ForceMode.Impulse);
            animator.SetTrigger("Jump");

        }
    }
}
