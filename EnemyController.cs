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

        if (!Trase(Player))
        {

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
                //Debug.Log(traseDirection + " " + traseDirection.magnitude);

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
        //Debug.Log(MaxHealth);

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

    void ShowBloodEffect(Collider other)
    {

        //Debug.Log(other.bounds);
        Vector3 pos = other.bounds.center;
        GameObject blood = Instantiate<GameObject>(BloodEffect, pos, other.transform.rotation);
        //blood.transform.SetParent(transform, false);
        Destroy(blood, 2f);

    }
}