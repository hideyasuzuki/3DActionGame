using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] RuntimeAnimatorController[] runtimeAnimatorController;
    [SerializeField] ChangeWeapon changeWeapon;
    [SerializeField] TpsCamera refCamera;
    [SerializeField] float soarTime = 0f;
    [SerializeField] GameObject[] hitPointArray;
    [SerializeField] AudioClip attack;
    [SerializeField] AudioClip damage;
    CharacterController characterController;
    Animator animator;
    AudioSource audioSource;
    Vector3 velocity;
    int runSpeed = 4;
    int walkSpeed = 2;
    int keyPush = 0;
    int moveParamHash = Animator.StringToHash("Move");
    int attackParamHash = Animator.StringToHash("Attack");
    int motionParamHash = Animator.StringToHash("MotionSpeed");
    int runRecovary;
    int hitPoint = 3;
    float apply = 0.05f;
    float motionSpeed = 1.0f;
    float tiredTime = 5.0f;
    float gravity = 10.0f;
    float runRecovaryTime = 8.0f;
    float runRecovaryTimer;
    float attackRecovary;
    float attackRecovaryTime = 5.0f;
    float attackRecovaryTimer;
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
    }

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
        if(isActive)
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
        velocity = Vector3.zero;

        if (Input.GetKey(KeyCode.LeftShift) && 
            !animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            Transfer(runSpeed, (int)PlayerAnimation.Run);
            Tired();
        }
        else if(!animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            Transfer(walkSpeed, (int)PlayerAnimation.Walk);
        }
        
        if (velocity.magnitude > 0)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(refCamera.HorizontalRotation * velocity),
                apply);
        }

        Attack();

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Idle") ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Walk"))
        {
            Recovary();
        }
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

        if (!Input.anyKey)
        {
            animator.SetInteger(moveParamHash, (int)PlayerAnimation.Idle);
        }

        velocity = velocity.normalized * speed * Time.deltaTime;
    }

    void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            keyPush++;
            Tired();
            animator.SetTrigger(attackParamHash);
        }
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
        if(col.gameObject.name == "kopie")
        {
            HitPoint();
            if(hitPoint <= 0)
            {
                isActive = false;
                animator.SetBool("Die", true);
            }
        }
    }

    void HitPoint()
    {
        hitPointArray[hitPoint - 1].SetActive(false);
        hitPoint--;
        audioSource.PlayOneShot(damage);
    }

    void AttackSound()
    {
        audioSource.PlayOneShot(attack);
    }
}
