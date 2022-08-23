using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public GameObject KeepPosition;

    public GameObject EquipPosition;

    GameObject CameraLockTarget;

    Camera Cam;

    Animator animator;

    Animator EnemeyAnim;

    public float WalkSpeed = 2f;

    public float RunSpeed = 6f;

    public float TurnSpeed = 10f;

    public bool IsAttacking = false;

    public float AttackSpeed = 0.3f;

    public bool CanInput = true;

    float Speed;

    bool CanAttack = true;

    bool WeaponEquipped = false;

    bool AlreadyAttacked = false;

    bool isCameraLock = false;


    void Start()
    {

        animator = GetComponent<Animator>();
        Cam = UnityEngine.Camera.main;
        Cursor.lockState = CursorLockMode.Locked;
        EquipPosition.SetActive(false);

        SetAbility(WalkSpeed, RunSpeed, TurnSpeed, AttackSpeed);


        GetCameraLockTarget();
        Debug.Log(CameraLockTarget.name);
    }


    void Update()
    {

        Attacked();

        Move(Cam.transform);

        DisableWeapon();

        AttackCoolDown();

        Attack();


    }


    void SetAbility(float WalkSpeed, float RunSpeed, float TurnSpeed, float AttackSpeed)
    {

        this.WalkSpeed = WalkSpeed;
        this.RunSpeed = RunSpeed;
        this.TurnSpeed = TurnSpeed;
        this.AttackSpeed = AttackSpeed;

    }


    void Move(Transform standard)
    {

        if (!CanInput)
        {

            //Debug.Log("cantmove");
            return;


        }


        Vector2 direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));


        bool isMove = direction.magnitude != 0;
        bool isRun = Input.GetKey(KeyCode.LeftShift);

        animator.SetBool("Walk", isMove);

        //Debug.Log(animator.GetBool("Walk"));

        if (isMove)
        {

            animator.SetBool("Run", isRun);


            Vector3 lookForward = new Vector3(standard.forward.x, 0, standard.forward.z).normalized;
            Vector3 lookRight = new Vector3(standard.right.x, 0, standard.right.z).normalized;

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

    }


    void GetCameraLockTarget()
    {

        Collider[] objects = Physics.OverlapSphere(transform.position, 20f);
        float distanceToPlayer = 20f;

        for (int i = 0; i < objects.Length; i++)
        {

            if (objects[i].gameObject.tag == "Enemy")
            {

                if(Vector3.Magnitude(objects[i].gameObject.transform.position - transform.position) <= distanceToPlayer)
                {

                    CameraLockTarget = objects[i].gameObject;
                    distanceToPlayer = Vector3.Magnitude(objects[i].gameObject.transform.position - transform.position);

                }

            }

        }

    }



    void CameraLock()
    {



    }



    void CameraLockMove()
    {



    }


    void DisableWeapon()
    {

        if (!CanInput)
        {

            return;

        }

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

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(2);

        if (Input.GetMouseButtonDown(0) && WeaponEquipped && CanAttack)
        {

            if (state.IsTag("Attack"))
            {

                animator.SetTrigger("NextAttack");

            }

            else if (!state.IsTag("Attack"))
            {

                animator.SetBool("Attack", true);
                SetIsAttacking(true);
                SetCanInput(false);

            }

        }

        if (state.IsTag("Attack") && state.normalizedTime >= 0.99)
        {

            animator.SetBool("Attack", false);
            AnimationEnd();
            CanAttack = false;

        }

    }


    void AttackCoolDown()
    {

        if (!CanAttack)
        {

            AttackSpeed -= Time.deltaTime;

            if (AttackSpeed <= 0)
            {

                AttackSpeed = 0.3f;
                CanAttack = true;

            }

        }

    }



    void SetCanInput(bool CanInput = true)
    {

        this.CanInput = CanInput;

    }


    void SetIsAttacking(bool isAttacking = false)
    {

        this.IsAttacking = isAttacking;

    }


    void Attacked()
    {

        if (animator.GetCurrentAnimatorStateInfo(3).IsTag("Attacked") &&
        animator.GetCurrentAnimatorStateInfo(3).normalizedTime >= 0.99f)
        {

            AlreadyAttacked = false;
            CanInput = true;

        }

    }




    void AnimationEnd()
    {

        SetCanInput();
        SetIsAttacking();

    }


    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "EnemyWeapon")
        {

            GameObject attackEnemy = other.gameObject;

            while (attackEnemy.transform.parent)
            {

                attackEnemy = attackEnemy.transform.parent.gameObject;

            }

            EnemeyAnim = attackEnemy.gameObject.GetComponent<Animator>();

            if (EnemeyAnim.GetCurrentAnimatorStateInfo(1).IsTag("Attack") && !AlreadyAttacked)
            {


                animator.SetTrigger("Attacked");
                CanInput = false;
                AlreadyAttacked = true;

            }

        }

    }



}

