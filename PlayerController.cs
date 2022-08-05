using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Animator animator;

    public GameObject KeepPosition;

    public GameObject EquipPosition;

    public Transform Cam;

    public float WalkSpeed = 2f;

    public float RunSpeed = 6f;

    public float TurnSpeed = 10f;

    float Speed;

    bool CanInput = true;

    bool WeaponEquipped = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        Cursor.lockState = CursorLockMode.Locked;
        EquipPosition.SetActive(false);

        SetAbility(WalkSpeed, RunSpeed);
    }

    void Update()
    {
        Move();

        DisableWeapon();

        if (WeaponEquipped)
        {

            Attack();

        }
    }

    void SetAbility(float WalkSpeed, float RunSpeed)
    {
        this.WalkSpeed = WalkSpeed;
        this.RunSpeed = RunSpeed;
    }

    void Move()
    {
        if (!CanInput)
        {
            //Debug.Log("cantmove");
            return;
        }

        Vector2 direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        bool isMove = direction.magnitude != 0;
        bool isRun = Input.GetKey(KeyCode.LeftShift);
        bool roll = Input.GetKeyDown(KeyCode.Space);


        animator.SetBool("Walk", isMove);



        if (isMove)
        {

            animator.SetBool("Run", isRun);


            Vector3 lookForward = new Vector3(Cam.forward.x, 0, Cam.forward.z).normalized;
            Vector3 lookRight = new Vector3(Cam.right.x, 0, Cam.right.z).normalized;

            Vector3 moveDir = lookForward * direction.y + lookRight * direction.x;
            transform.forward = moveDir;

            if (isRun)
            {

                Speed = RunSpeed;

            }
            else
            {

                Speed = WalkSpeed;

            }

            transform.position += moveDir * Speed * Time.deltaTime;

        }
        else
        {
            if (animator.GetBool("Run"))
            {

                animator.SetBool("Run", false);

            }
        }

        if (roll && isMove || roll && isRun)
        {

            animator.SetTrigger("Roll");

        }
    }

    void DisableWeapon()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (WeaponEquipped)
            {

                SetCanInput(false);
                animator.SetTrigger("WeaponEquip");

            }
            else
            {

                SetCanInput(false);
                animator.SetTrigger("WeaponEquip");

            }
        }
    }

    void AbleWeapon()
    {
        if (WeaponEquipped)
        {

            WeaponEquipped = false;
            animator.SetBool("ReadyCombat", false);
            EquipPosition.SetActive(false);
            KeepPosition.SetActive(true);

        }
        else
        {

            WeaponEquipped = true;
            animator.SetBool("ReadyCombat", true);
            EquipPosition.SetActive(true);
            KeepPosition.SetActive(false);

        }
    }

    void Attack()
    {

        if (Input.GetMouseButtonDown(0))
        {

            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

            if (state.IsTag("Attack") && state.normalizedTime >= 0.3f)
            {

                animator.SetTrigger("Attack");
                SetCanInput(false);

            }
            else if (!state.IsTag("Attack"))
            {

                animator.SetTrigger("Attack");
                SetCanInput(false);

            }
        }
    }

    void SetCanInput(bool CanInput = true)
    {
        this.CanInput = CanInput;
    }

    void AnimationEnd()
    {
        SetCanInput();
    }

}

