using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRoll : MonoBehaviour
{

    Animator animator;

    Rigidbody rigid;

    PlayerController playerController;

    public float RollPower = 5f;

    bool Rolling = false;
    
    void Start()
    {

        animator = GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>();
        playerController = GetComponent<PlayerController>();
    
    }

    
    void Update()
    {

        Roll();
        
        CheckRollingFinish();
    
    }

    void Roll()
    {
        
        if(Input.GetKeyDown(KeyCode.Space) && !Rolling)
        {

            playerController.SetCanInput(false);

            Rolling = true;
            animator.SetTrigger("Roll");
            rigid.AddForce(new Vector3(0,0,1) * RollPower, ForceMode.Impulse);

        }
    
    }

    void CheckRollingFinish()
    {

        if(animator.GetCurrentAnimatorStateInfo(1).IsTag("Roll") && animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= 0.99f)
        {

            playerController.SetCanInput(true);
            
            Rolling = false;
            //Debug.Log("Roll finish");

        }

    }

}
