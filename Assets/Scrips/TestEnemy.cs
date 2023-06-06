using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class TestEnemy : MonoBehaviour
{
    [SerializeField] AudioClip attack;
    [SerializeField] AudioClip damage;
    NavMeshAgent agent;
    GameObject player;
    Animator animator;
    AudioSource audioSource;
    Vector3 playerPos;
    int moveParamHash = Animator.StringToHash("Move");
    int hitPoint = 4;
    float distance;
    float trackingRange = 4f;
    float attackRange = 1.2f;

    enum GoblinAnimation
    {
        Idle,
        Run,
        Attack,
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        player = GameObject.Find("Set Costume_02 SD Unity-Chan WGS");
    }

    void Update()
    {
        playerPos = player.transform.position;
        distance = Vector3.Distance(playerPos,this.transform.position);
        Range();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Weapon")
        {
            hitPoint--;
            audioSource.PlayOneShot(damage);
            animator.SetBool("Damage", true);
            if(hitPoint <= 0)
            {
                animator.SetBool("Death", true);
                trackingRange = 0;
                agent.updatePosition = false;
            }
        }
    }

    void Range()
    {
        if (distance < trackingRange)
        {
            if (distance < attackRange)
            {
                animator.SetInteger(moveParamHash, (int)GoblinAnimation.Attack);
            }
            else
            {
                agent.updatePosition = true;
                agent.destination = playerPos;
                transform.LookAt(playerPos);
                animator.SetInteger(moveParamHash, (int)GoblinAnimation.Run);
            }
        }
        else
        {
            agent.updatePosition = false;
            animator.SetInteger(moveParamHash, (int)GoblinAnimation.Idle);
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            agent.updatePosition = false;
            agent.nextPosition = transform.position;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Damage"))
        {
            animator.SetBool("Damage", false);
        }
    }

    void attackSound()
    {
        audioSource.PlayOneShot(attack);
    }

    public void ObjDestroy()
    {
        Destroy(gameObject);
    }
}
