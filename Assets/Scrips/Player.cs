using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] RuntimeAnimatorController[] runtimeAnimatorController = null;
    [SerializeField] ChangeWeapon changeWeapon = null;
    [SerializeField] TpsCamera refCamera = null;
    [SerializeField] GameObject[] hitPointArray = null;
    [SerializeField] AudioClip attack = null;
    [SerializeField] AudioClip damage = null;
    [SerializeField] float soarTime = 0f;
    CharacterController characterController;
    Animator animator = null;
    AudioSource audioSource = null;
    Vector3 velocity = Vector3.zero;
    int runSpeed = 4;
    int walkSpeed = 2;
    int keyPush = 0;
    int moveParamHash = Animator.StringToHash("Move");
    int attackParamHash = Animator.StringToHash("attack");
    int AttackParamHash = Animator.StringToHash("Attack");
    int motionParamHash = Animator.StringToHash("MotionSpeed");
    int runRecovary;
    int hitPoint = 3;
    float apply = 0.05f;
    float motionSpeed = 1.0f;
    float tiredTime = 5.0f;
    float gravity = 10.0f;
    float runRecovaryTime = 8.0f;
    float runRecovaryTimer = 0.0f;
    float attackRecovary = 0.0f;
    float attackRecovaryTime = 5.0f;
    float attackRecovaryTimer = 0.0f;
    bool isActive = true;

    public bool IsActive
    {
        get { return isActive; }
    }

    public Animator Animator
    {
        get { return animator; }
    }

    enum PlayerAnimation
    {
        Idle,
        Walk,
        Run,
        Attack,
    }

    enum PlayerState
    {
        Idle,
        Transfer,
        Attack,
        ReceiveDamage,
        Die,
    }

    PlayerState playerState = PlayerState.Idle;

    void Start()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
        runRecovary = runSpeed;
        attackRecovary = motionSpeed;
    }

    void Update()
    {
        velocity = Vector3.zero;

        if (isActive)
        {
            Controller();
        }       
       
        velocity.y -= gravity * Time.deltaTime;
        characterController.Move(refCamera.HorizontalRotation * velocity);
        if (changeWeapon.IsChange)
        {
            animator.runtimeAnimatorController = runtimeAnimatorController[0];
        }
    }

    void Controller()
    {
        if (!Input.anyKey)
        {
            playerState = PlayerState.Idle;
        }

        switch (playerState)
        {
            case PlayerState.Idle:
                Idle();
                Recovary();
                break;
            case PlayerState.Transfer:
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    Transfer(runSpeed, (int)PlayerAnimation.Run);
                    Tired();
                    break;
                }
                Transfer(walkSpeed, (int)PlayerAnimation.Walk);
                Recovary();
                break;
            case PlayerState.Attack:
                Attack();
                break;
            case PlayerState.ReceiveDamage:
                break;
            case PlayerState.Die:
                break;
        }

        if (velocity.magnitude > 0)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(refCamera.HorizontalRotation * velocity),
                apply);
        }
    }

    void Idle()
    {
        if (Input.GetMouseButtonDown(0))
        {
            playerState = PlayerState.Attack;
            return;
        }
        else if (Input.anyKey)
        {
            playerState = PlayerState.Transfer;
            return;
        }
        
        animator.SetInteger(moveParamHash, (int)PlayerAnimation.Idle);
    }

    void Transfer(int speed, int animationParam)
    {
        if (Input.GetKey(KeyCode.W))
        {
            velocity.z += speed;
            animator.SetInteger(moveParamHash, animationParam);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            velocity.z -= speed;
            animator.SetInteger(moveParamHash, animationParam);
        }

        if (Input.GetKey(KeyCode.D))
        {
            velocity.x += speed;
            animator.SetInteger(moveParamHash, animationParam);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            velocity.x -= speed;
            animator.SetInteger(moveParamHash, animationParam);
        }

        if(Input.GetMouseButton(0))
        {
            playerState = PlayerState.Attack;
        }
        else if(!Input.anyKey)
        {
            animator.SetInteger(moveParamHash, (int)PlayerAnimation.Idle);
        }

        velocity = velocity.normalized * speed * Time.deltaTime;
    }

    void Attack()
    {
        Tired();
        animator.SetTrigger(AttackParamHash);
    }

    void Tired()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
        {
            soarTime += Time.deltaTime;
            if (soarTime > tiredTime)
            {
                if (walkSpeed >= runSpeed)
                {
                    return;
                }

                runSpeed--;
                soarTime = 0;
            }
        }
        
        if(animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            if(keyPush >= 5)
            {
                if (motionSpeed > 0.6)
                {
                    motionSpeed -= 0.2f;
                }

                keyPush = 0;
            }
        }

        animator.SetFloat(motionParamHash, motionSpeed);
    }

    void Recovary()
    {
        if(runRecovary > runSpeed)
        {
            runRecovaryTimer += Time.deltaTime;

            if (runRecovaryTimer > runRecovaryTime)
            {
                runSpeed++;
                runRecovaryTimer = 0;
            } 
        }

        if(attackRecovary > motionSpeed)
        {
            attackRecovaryTimer += Time.deltaTime;

            if(attackRecovaryTimer > attackRecovaryTime)
            {
                motionSpeed += 0.2f;
                keyPush = 0;
                attackRecovaryTimer = 0;
            }
        }

    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "EnemyWeapon")
        {
            HitPoint();
            if (hitPoint <= 0)
            {
                isActive = false;
                animator.SetBool("Die", true);
            }
        }
    }

    void HitPoint()
    {
        if(hitPointArray != null)
        {
            hitPointArray[hitPoint - 1].SetActive(false);
            hitPoint--;
            audioSource.PlayOneShot(damage);
        }   
    }

    void AttackVoiceSound()
    {
        audioSource.PlayOneShot(attack);
    }
}
