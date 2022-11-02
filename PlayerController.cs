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

    Animator EnemyAnim;


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

        /*
        화면 고정이 활성화 되었고, 고정할 타겟이 존재하다면
        화면 고정 지점의 위치를 업데이트하고,
        플레이어가 그 지점을 기준으로 움직임
        */
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

    /*
    기능: 시작시 기본 능력치 설정
    */
    void SetAbility(float WalkSpeed, float RunSpeed, float TurnSpeed, float AttackSpeed)
    {

        this.WalkSpeed = WalkSpeed;
        this.RunSpeed = RunSpeed;
        this.TurnSpeed = TurnSpeed;
        this.AttackSpeed = AttackSpeed;

    }

    /*
    기능: 플레이어 이동
    설명: Transform 변수를 기준으로 플레이어를 이동시킴
    */
    void Move(Transform standard)
    {

        //특정 행동으로 인해 키보드 입력을 받을 수 없는 상태의 경우 return
        if (!CanInput)
        {
            
            return;

        }

        //키보드 입력값을 벡터로 저장
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));


        //카메라 고정상태라면 다른 애니메이션 상태값을 설정하고 상대좌표로 위치 갱신
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

            //direction벡터의 길이가 0인지 판단 (방향키를 입력 받았는지 여부 판단)
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

                //움직임의 방향을 standard 게임 오브젝트를 기준으로 수정
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

    /*
    기능: 카메라 고정
    설명:
    카메라 고정o: 적의 체력과 시점 고정 UI 비활성화 후 카메라 고정 해제 
    카메라 고정x: CameraLockTarget 오브젝트 탐색 후 카메라 고정
    */
    void CameraLock()
    {

        if (Input.GetKeyDown(KeyCode.Q))
        {
            //카메라 고정 상태
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
            //카메라 고정 상태x
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


    /*
    기능: 카메라 고정 타겟 탐색
    설명:
    플레이어와 일정 거리 내에 있는 collider 컴포넌트가 존재하는 게임 오브젝트 탐색,
    tag가 Enemy인 오브젝트 중 플레이어와의 거리가 가장 짧은 오브젝트를 타겟으로 설정
    */
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


    /*
    기능: 카메라 고정 기준 오브젝트의 rotation 업데이트
    설명:
    카메라가 추적 할 오브젝트가 플레이어를 향하도록 rotation업데이트
    */
    void CameraLockStandardUpdate()
    {

        Vector3 goal = CameraLockTarget.transform.position - this.transform.position;

        goal.Normalize();

        CameraLockStandard.transform.forward = goal;

    }


    /*
    기능: 무기해제
    설명:
    키보드 E입력 시 다른 키 입력을 비활성화하고 무기 장착/해제 애니메이션 실행 
    */
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

    /*
    기능: 무기 활성화
    설명: 무기 장착 여부를 판단하여 무기의 위치를 변경
    */
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

    /*
    기능: 공격
    설명:
    마우스 왼쪽 버튼 클릭 시 공격 애니메이션 실행,
    만약 공격 애니메이션 실행 중 입력이 다시 들어왔을 경우 다음 공격 애니메이션 실행
    */
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

    /*
    기능: 공격속도
    설명:
    공격 애니메이션이 종료 된 후 다음 공격까지의 쿨타임 감소
    */
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

    /*
    기능: 피격
    설명:
    피격 애니메이션 종료 시 다음 피격이 가능하도록 설정
    */
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



    /*
    기능: 애니메이션 종료
    설명:
    애니메이션 종료시 입력이 가능하도록 설정
    */
    void AnimationEnd()
    {

        CanInput = true;
        IsAttacking = false;

    }


    /*
    기능: 피격처리2
    설명:
    적의 무기의 TriggerEnter가 발생했을 시 적의 애니메이션을 확인하여 공격 모션일 시 피격판정처리
    */
    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "EnemyWeapon")
        {

            GameObject attackEnemy = other.gameObject;

            while (attackEnemy.transform.parent)
            {

                attackEnemy = attackEnemy.transform.parent.gameObject;

            }

            EnemyAnim = attackEnemy.gameObject.GetComponent<Animator>();

            if (EnemyAnim.GetCurrentAnimatorStateInfo(0).IsTag("Attack") && !AlreadyAttacked)
            {

                animator.SetTrigger("Attacked");
                CanInput = false;
                CanAttack = false;
                AlreadyAttacked = true;

            }

        }

    }



}

