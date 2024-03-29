using UnityEngine;
using UnityEngine.UI;
public class EnemyController : MonoBehaviour
{

    public GameObject CameraLockPos;

    GameObject Player;

    GameObject PrfHpBar;

    GameObject Canvas;

    GameObject BloodEffect;

    GameObject PrfCameraLockDot;

    RectTransform HpBar;

    Image CameraLockDot;

    Camera Cam;

    Animator animator;

    Animator playerAnim;

    PlayerController playerController;

    Image CurrentHpBar;


    public Vector3 CameraLockOffSet;


    Vector3 MoveDirection;


    float MoveSpeed;

    float DirectionChangeTime;

    float MaxHealth = 20;

    float DetectRange = 15f;

    float TraseSpeed = 0.5f;

    float AttackRange = 2.5f;

    float AttackCoolDownReset = 5f;

    bool ShowingCameraLockUI = false;

    float Health;

    float AttackCoolDown;

    bool AlreadytHit = false;


    public RectTransform GetHpBar()
    {

        return this.HpBar;

    }

    public Image GetCameraLockDot()
    {

        return this.CameraLockDot;

    }

    public void SetShowingCameraLockUI(bool condition)
    {

        this.ShowingCameraLockUI = condition;

    }


    void Start()
    {

        Player = GameObject.Find("Player");
        
        Canvas = GameObject.Find("Canvas");

        Cam = UnityEngine.Camera.main;

        playerController = Player.GetComponent<PlayerController>();

        BloodEffect = Resources.Load<GameObject>("BloodEffect");

        PrfHpBar = Resources.Load<GameObject>("BgHpBar");

        PrfCameraLockDot = Resources.Load<GameObject>("CameraLockDot");

        animator = GetComponent<Animator>();

        playerAnim = Player.GetComponent<Animator>();

        MoveDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

        HpBar = Instantiate(PrfHpBar, Canvas.transform).GetComponent<RectTransform>();

        CameraLockDot = Instantiate(PrfCameraLockDot, Canvas.transform).GetComponent<Image>();

        CurrentHpBar = HpBar.transform.GetChild(0).GetComponent<Image>();

        HpBar.gameObject.SetActive(false);

        CameraLockDot.gameObject.SetActive(false);

        SetAbility(Random.Range(0, 4), Random.Range(2, 10), MaxHealth, DetectRange, TraseSpeed, AttackRange, AttackCoolDownReset);


    }


    void SetAbility(float moveSpeed, float directionChangeTime, float maxHealth, float detectRange, float traseSpeed, float attackRange, float attackCoolDownReset)
    {

        this.MoveSpeed = moveSpeed;
        this.DirectionChangeTime = directionChangeTime;
        this.Health = maxHealth;
        this.DetectRange = detectRange;
        this.TraseSpeed = traseSpeed;
        this.AttackRange = attackRange;
        this.AttackCoolDown = attackCoolDownReset;

    }


    void Update()
    {

        Attacked();

        //플레이어가 Trase범위 내에 있는지 판단
        if (!Trase(Player))
        {
            //특정 시간이 지날때마다 이동 방향과 속도 랜덤으로 재설정
            if (Timer(ref DirectionChangeTime) <= 0)
            {

                ResetTimer(ref DirectionChangeTime, Random.Range(2, 10));
                SetDirection(ref MoveDirection);

            }


            Move(MoveDirection, MoveSpeed);

        }
        
        if (ShowingCameraLockUI)
        {

            ShowHpBar();
            ShowCameraLockDot();

        }


    }

    float Timer(ref float time)
    {

        time -= Time.deltaTime;

        return time;

    }

    void ResetTimer(ref float time, float resetTime)
    {

        time = resetTime;

    }

    void SetDirection(ref Vector3 moveDirection)
    {

        moveDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        MoveSpeed = Random.Range(0, 4);

    }

    void ShowHpBar()
    {

        Vector3 hpBarPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + 2f, transform.position.z));
        HpBar.position = hpBarPos;
        CurrentHpBar.fillAmount = Health / MaxHealth;

    }


    void ShowCameraLockDot()
    {

        Vector3 dotPos = Camera.main.WorldToScreenPoint(CameraLockPos.transform.position + CameraLockOffSet);
        CameraLockDot.transform.position = dotPos;
        
    }


    /*
    기능: 이동
    설명:
    방향과 속도를 입력받고 해당 방향으로 이동
    */
    void Move(Vector3 moveDirection, float moveSpeed)
    {

        transform.forward = moveDirection;

        if (moveSpeed == 0 || Mathf.Approximately(moveDirection.sqrMagnitude, 0f))
        {

            animator.SetBool("Walk", false);
            return;

        }
        else
        {

            animator.SetBool("Walk", true);

        }

        transform.position += moveDirection * moveSpeed * Time.deltaTime;

    }

    /*
    기능: 추적
    설명:
    인자로 받은 게임 오브젝트를 추격함
    추격이 가능할 시 combat 애니메이션을 실행
    */
    bool Trase(GameObject target)
    {

        float distance = Vector3.Magnitude(transform.position - target.transform.position);


        if (distance < DetectRange)
        {

            Timer(ref AttackCoolDown);
            Vector3 traseDirection = target.transform.position - transform.position;

            if (distance > AttackRange)
            {

                animator.SetBool("Combat", true);
                Move(traseDirection, TraseSpeed);

            }
            else
            {

                animator.SetBool("Combat", true);

                Move(traseDirection, 0);

                if (AttackCoolDown <= 0)
                {

                    Attack();
                    ResetTimer(ref AttackCoolDown, AttackCoolDownReset);

                }

                return true;


            }

            return true;

        }
        else
        {

            animator.SetBool("Combat", false);
            return false;

        }

    }

    /*
    기능: 공격
    설명:
    4가지의 공격 모션 중 하나 실행
    */
    void Attack()
    {
        
        int attackNum = Random.Range(1, 5);
        animator.SetTrigger("Attack" + attackNum.ToString());

    }




    void getDamage(float damage)
    {

        Health -= damage;

        if (Health <= 0)
        {

            Die();

        }

    }


    void Die()
    {

        if(System.Object.ReferenceEquals(this.gameObject, playerController.GetCameraLockTarget()))
        {

            playerController.SetIsCameraLock(false);
            playerAnim.SetBool("CameraLock", false);

        }

        Destroy(gameObject);
        Destroy(HpBar.gameObject);
        Destroy(CameraLockDot.gameObject);

    }


    void Attacked()
    {

        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Attacked"))
        {

            MoveSpeed = 0;

        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Attacked") &&
        animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
        {

            AlreadytHit = false;

        }

    }

    /*
    기능: 피격처리
    설명:
    플레이어의 무기에 공격을 받았을 시 출혈 파티클 생성, 체력 감소, 피격 애니메이션 실행
    */
    void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag == "PlayerWeapon" && !AlreadytHit && Player.GetComponent<PlayerController>().GetIsAttacking()
        && !animator.GetCurrentAnimatorStateInfo(0).IsTag("Attacked"))
        {
            
            if (playerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f
            && playerAnim.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.9f)
            {
                
                if (playerAnim.GetCurrentAnimatorStateInfo(0).IsName("FinishAttack"))
                {

                    animator.SetTrigger("AttackedLargeMotion");

                }
                else
                {

                    animator.SetTrigger("Attacked");

                }
                
                AlreadytHit = true;
                getDamage(2);
                ShowBloodEffect(other);

            }

        }

    }
    
    /*
    기능: 출혈 파티클 생성
    설명:
    피격 시 해당 위치에 출혈 파티클을 2초동안 생성하여 유지
    */
    void ShowBloodEffect(Collider other)
    {

        Vector3 pos = other.bounds.center;
        GameObject blood = Instantiate<GameObject>(BloodEffect, pos, other.transform.rotation);
        Destroy(blood, 2f);

    }
}