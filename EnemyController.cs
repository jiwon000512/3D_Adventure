using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{

    public GameObject Player;

    Animator animator;

    Animator playerAnim;

    Vector3 Direction;

    public float MoveSpeed;

    public float DirectionChangeTime;

    public float MaxHealth;

    bool AlreadytHit = false;


    void Start()
    {

        animator = GetComponent<Animator>();

        playerAnim = Player.GetComponent<Animator>();

        Direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));

        SetAbility(Random.Range(0f, 3f), Random.Range(2, 10), 10);

    }

    void SetAbility(float MoveSpeed, float DirectionChangeTime, float MaxHealth)
    {

        this.MoveSpeed = MoveSpeed;
        this.DirectionChangeTime = DirectionChangeTime;
        this.MaxHealth = MaxHealth;


    }


    void Update()
    {

        Attacked();
        Move();

    }

    void Move()
    {
        DirectionChangeTime -= Time.deltaTime;


        if (DirectionChangeTime <= 0)
        {

            DirectionChangeTime = Random.Range(2, 10);
            Direction = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
            MoveSpeed = Random.Range(1f, 3f);

        }

        transform.forward = Direction;

        if (MoveSpeed == 0 || Mathf.Approximately(Direction.sqrMagnitude, 0f))
        {

            animator.SetBool("Walk", false);
            animator.speed = 1f;
            return;

        }
        else
        {

            animator.SetBool("Walk", true);
            animator.speed = 0.4f * MoveSpeed;

        }


        transform.position += Direction * MoveSpeed * Time.deltaTime;

    }




    void getDamage(float damage)
    {

        MaxHealth -= damage;

        if (MaxHealth <= 0)
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
        animator.GetCurrentAnimatorStateInfo(1).normalizedTime >= 0.6f)
        {

            AlreadytHit = false;

        }

    }

    void OnTriggerStay(Collider other)
    {

        if (other.gameObject.tag == "PlayerWeapon" && !AlreadytHit && Player.GetComponent<PlayerController>().IsAttacking)
        {

            if (playerAnim.GetCurrentAnimatorStateInfo(2).normalizedTime <= 0.7f
            && playerAnim.GetCurrentAnimatorStateInfo(2).normalizedTime >= 0.2f)
            {

                animator.SetTrigger("Attacked");
                AlreadytHit = true;
                getDamage(2);


            }

        }

    }
}