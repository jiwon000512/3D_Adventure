using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponEffect : MonoBehaviour
{
    
    GameObject Target;

    GameObject Effect;

    Animator TargetAnim;


    void Start()
    {

        Effect = this.transform.GetChild(0).gameObject;

        Target = this.gameObject;
        while(!Target.gameObject.GetComponent<Animator>())
        {

            Target = Target.transform.parent.gameObject;

        }

        TargetAnim = Target.GetComponent<Animator>();
        
        Effect.SetActive(false);
        
        
    }

    void Update()
    {

        CheckAttacking();

    }

    void CheckAttacking()
    {

        
        if(TargetAnim.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {

            if(TargetAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.5f 
            && TargetAnim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.8f)
            {

                Effect.SetActive(true);

            }

            if(TargetAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
            {
                
                Effect.SetActive(false);

            }

        }
    

    }
}
