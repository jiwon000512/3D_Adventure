using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public GameObject Player;


    GameObject BloodEffect;

    Animator animator;

    Animator playerAnim;


    Vector3 MoveDirection;


    public float MoveSpeed;

    public float DirectionChangeTime;

    public float MaxHealth = 20;

    public float DetectRange = 15f;

    public float TraseSpeed = 0.5f;

    public float AttackRange = 2.5f;

    public float AttackCoolDownReset = 5f;

    float Health;    
    
    float AttackCoolDown;

    bool AlreadytHit = false;


    void Start()
    {

        Player = GameObject.Find("Player");

        BloodEffect = Resources.Load<GameObject>("BloodEffect");

        animator = GetComponent<Animator>();

        playerAnim = Player.GetComponent<Animator>();

        MoveDirection = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

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

                    if(AttackCoolDown <= 0)
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

        int attackNum = Random.Range(1,5);
        animator.SetTrigger("Attack" + attackNum.ToString());

    }




    void getDamage(float damage)
    {

        Health -= damage;

        if (Health <= 0)
        {

            Dead();

        }
        //Debug.Log(MaxHealth);

    }


    void Dead()
    {

        Destroy(gameObject);

    }


    void Attacked()
    {

        if (animator.GetCurrentAnimatorStateInfo(1).IsTag("Attacked"))
        {

            MoveSpeed = 0;

        }

        if (animator.GetCurrentAnimatorStateInfo(1).IsTag("Attacked") &&
        animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= 0.99f)
        {

            AlreadytHit = false;

        }

    }

    void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag == "PlayerWeapon" && !AlreadytHit && Player.GetComponent<PlayerController>().IsAttacking)
        {

            if (playerAnim.GetCurrentAnimatorStateInfo(2).normalizedTime >= 0.4f
            && playerAnim.GetCurrentAnimatorStateInfo(2).normalizedTime <= 0.8f)
            {

                if(playerAnim.GetCurrentAnimatorStateInfo(2).IsName("FinishAttack"))
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