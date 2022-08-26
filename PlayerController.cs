using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public GameObject KeepPosition;

    public GameObject EquipPosition;

    GameObject CameraLockTarget;

    GameObject CameraLockStandard;


    Camera Cam;

    EnemyController enemyController;

    Animator animator;

    Animator EnemeyAnim;


    float WalkSpeed = 2f;

    float RunSpeed = 6f;

    float TurnSpeed = 10f;

    float AttackSpeed = 0.3f;

    bool IsAttacking = false;

    bool CanInput = true;

    bool IsCameraLock = false;

    float Speed;

    bool CanAttack = true;

    bool WeaponEquipped = false;

    bool AlreadyAttacked = false;


    public GameObject GetCameraLockTarget()
    {

        return this.CameraLockTarget;

    }

    public GameObject GetCameraLockStandard()
    {

        return this.CameraLockStandard;

    }


    public void SetIsCameraLock(bool condition)
    {

        this.IsCameraLock = condition;

    }

    public bool GetIsCameraLock()
    {

        return this.IsCameraLock;

    }

    public bool GetIsAttacking()
    {

        return this.IsAttacking;

    }

    public void SetCanInput(bool condition)
    {

        this.CanInput = condition;

    }


    void Start()
    {

        animator = GetComponent<Animator>();

        Cam = UnityEngine.Camera.main;

        Cursor.lockState = CursorLockMode.Locked;

        EquipPosition.SetActive(false);

        SetAbility(WalkSpeed, RunSpeed, TurnSpeed, AttackSpeed);

    }


    void Update()
    {

        Attacked();

        CameraLock();

        if (IsCameraLock && CameraLockTarget != null)
        {

            CameraLockStandardUpdate();
            Move(CameraLockStandard.transform);

        }
        else
        {

            Move(Cam.transform);

        }


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
            
            //Debug.Log("Can't Input Key!");
            return;

        }


        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (IsCameraLock)
        {

            animator.SetFloat("CLMoveForward", direction.z);
            animator.SetFloat("CLMoveRight", direction.x);

            Vector3 forwardDirection = standard.position - this.transform.position;
            forwardDirection.y = 0;

            transform.forward = Vector3.Lerp(transform.forward, forwardDirection, TurnSpeed * Time.deltaTime);
            transform.Translate(direction * Speed * Time.deltaTime);

        }

        else
        {

            bool isMove = direction.magnitude != 0;
            bool isRun = Input.GetKey(KeyCode.LeftShift);

            animator.SetBool("Walk", isMove);

            //Debug.Log(animator.GetBool("Walk"));

            if (isMove)
            {

                animator.SetBool("Run", isRun);

                if (isRun)
                {

                    Speed = RunSpeed;

                }
                else
                {

                    Speed = WalkSpeed;

                }


                Vector3 lookForward = new Vector3(standard.forward.x, 0, standard.forward.z).normalized;
                Vector3 lookRight = new Vector3(standard.right.x, 0, standard.right.z).normalized;
                Vector3 moveDir = lookForward * direction.z + lookRight * direction.x;

                transform.forward = moveDir;
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


    }


    void CameraLock()
    {

        if (Input.GetKeyDown(KeyCode.Q))
        {

            if (IsCameraLock)
            {

                enemyController = CameraLockTarget.GetComponent<EnemyController>();
                enemyController.GetHpBar().gameObject.SetActive(false);
                enemyController.GetCameraLockDot().gameObject.SetActive(false);
                enemyController.SetShowingCameraLockUI(false);

                Destroy(CameraLockStandard);
                CameraLockTarget = null;
                IsCameraLock = !IsCameraLock;
                animator.SetBool("CameraLock", false);

            }
            else
            {

                SetCameraLockTarget();

                if (CameraLockTarget != null)
                {
                    
                    IsCameraLock = !IsCameraLock;
                    animator.SetBool("CameraLock", true);

                }

            }

        }

    }



    void SetCameraLockTarget()
    {

        float distanceToPlayer = 10f;
        Collider[] objects = Physics.OverlapSphere(transform.position, distanceToPlayer);

        for (int i = 0; i < objects.Length; i++)
        {

            if (objects[i].gameObject.tag == "Enemy")
            {

                if (Vector3.Magnitude(objects[i].gameObject.transform.position - transform.position) <= distanceToPlayer)
                {

                    CameraLockTarget = objects[i].gameObject;
                    distanceToPlayer = Vector3.Magnitude(objects[i].gameObject.transform.position - transform.position);

                }

            }

        }

        if (CameraLockTarget != null)
        {

            enemyController = CameraLockTarget.GetComponent<EnemyController>();

            enemyController.SetShowingCameraLockUI(true);

            enemyController.GetHpBar().gameObject.SetActive(true);

            enemyController.GetCameraLockDot().gameObject.SetActive(true);

            CameraLockStandard = new GameObject("CameraLockStandard");

            CameraLockStandard.transform.position = CameraLockTarget.transform.position + new Vector3(0, 1f, 0);

            CameraLockStandard.transform.LookAt(CameraLockStandard.transform);

            CameraLockStandard.transform.parent = CameraLockTarget.transform;



        }



    }



    void CameraLockStandardUpdate()
    {

        Vector3 goal = CameraLockTarget.transform.position - this.transform.position;

        goal.Normalize();

        //Debug.DrawLine(Vector3.zero, goal, Color.red);

        CameraLockStandard.transform.forward = goal;

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

                CanInput = false;
                animator.SetTrigger("WeaponEquip");

            }
            else
            {

                CanInput = false;
                animator.SetTrigger("WeaponEquip");

            }

        }

    }


    public void AbleWeapon()
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

        AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);

        if (Input.GetMouseButtonDown(0) && WeaponEquipped && CanAttack && !state.IsTag("Attacked"))
        {

            animator.SetTrigger("Attack");
            IsAttacking = true;
            CanInput = false;

        }

        if (state.IsTag("Attack") && state.normalizedTime >= 0.99)
        {

            AnimationEnd();
            CanAttack = false;
            if(state.IsName("FinishAttack"))
            {

                animator.ResetTrigger("Attack");

            }

        }

    }


    void AttackCoolDown()
    {

        if (!CanAttack)
        {

            AttackSpeed -= Time.deltaTime;

            if (AttackSpeed <= 0)
            {

                AttackSpeed = 0.5f;
                CanAttack = true;

            }

        }

    }



    void Attacked()
    {

        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Attacked") &&
        animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
        {

            AlreadyAttacked = false;
            CanInput = true;
            CanAttack = true;
            animator.ResetTrigger("Attack");

        }

    }




    void AnimationEnd()
    {

        CanInput = true;
        IsAttacking = false;

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
                CanAttack = false;
                AlreadyAttacked = true;

            }

        }

    }



}

