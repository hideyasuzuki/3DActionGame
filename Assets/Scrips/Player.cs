using System;
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
    [SerializeField] AudioClip attackSound = null;
    [SerializeField] AudioClip damageSound = null;
    CharacterController characterController = null;
    Animator animator = null;
    AudioSource audioSource = null;
    /// <summary>
    /// Player�̑��x
    /// </summary>
    Vector3 velocity = Vector3.zero;
    /// <summary>
    /// ���鑬��
    /// </summary>
    public int runSpeed = 4;
    /// <summary>
    /// ��������
    /// </summary>
    int walkSpeed = 2;
    /// <summary>
    /// �A�j���[�V������Parameters��id
    /// </summary>
    int moveParamHash = Animator.StringToHash("Move");
    /// <summary>
    /// �A�j���[�V������Parameters��id
    /// </summary>
    int attackParamHash = Animator.StringToHash("Attack");
    /// <summary>
    /// �A�j���[�V������Parameters��id
    /// </summary>
    int motionParamHash = Animator.StringToHash("MotionSpeed");
    /// <summary>
    /// ��J���~�ς���
    /// </summary>
    int accumulateFatigue = 0;
    int hitPoint = 3;
    float strengthRecoveryTimer = 0.0f;
    float attackRecoveryTimer = 0.0f;
    float accumulationTimer = 0f;
    /// <summary>
    /// �U������̑���
    /// </summary>
    float apply = 0.05f;
    /// <summary>
    /// �A�j���[�V�����Đ��̑���
    /// </summary>
    float motionSpeed = 1.0f;
    float hitStopTime = 0.1f;
    /// <summary>
    /// Player�̃A�N�e�B�u��Ԃ̕ύX
    /// </summary>
    bool isActive = true;

    public bool IsActive
    {
        get { return isActive; }
    }
    
    /// <summary>
    /// Player�̃A�j���[�V����id
    /// </summary>
    enum PlayerAnimation
    {
        Idle,
        Walk,
        Run,
        Attack,
    }

    /// <summary>
    /// Player�̏�ԑJ��id
    /// </summary>
    enum PlayerState
    {
        Idle,
        Walk,
        Run,
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
    }

    void Update()
    {
        //�O�t���[���̑��x�̒l�������Ă��邽�ߒl������������
        velocity = Vector3.zero;
        //���킪�ύX���ꂽ��A�j���[�V������ύX����
        if (changeWeapon.IsChange && animator.runtimeAnimatorController != runtimeAnimatorController[0])
        {
            animator.runtimeAnimatorController = runtimeAnimatorController[0];
            animator.SetFloat(motionParamHash, motionSpeed);
        }

        //��A�N�e�B�u�̎�Player�������Ȃ��悤�ɂ���
        if (isActive)
        {
            Controller();
        }

        float gravity = 10.0f;
        //velocity.y -= gravity * Time.deltaTime;
        //�J�����̌����Ɉړ�������
        characterController.Move(refCamera.HorizontalRotation * velocity);
    }

    void Controller()
    {
        //Player�̏�ԑJ��
        switch (playerState)
        {
            case PlayerState.Idle:
                Idle();
                Debug.Log(playerState);
                Recovery();
                break;
            case PlayerState.Walk:
                Transfer(walkSpeed, (int)PlayerAnimation.Walk);
                Recovery();
                break;
            case PlayerState.Run:
                Transfer(runSpeed, (int)PlayerAnimation.Run);
                Tired();
                break;
            case PlayerState.Attack:
                Debug.Log(playerState);
                Attack();
                break;
            case PlayerState.ReceiveDamage:
                break;
            case PlayerState.Die:
                break;
        }

        ChangeState(playerState);

        //�ړ����Ă���Ȃ�Player���ړ����Ă������������
        if (velocity.magnitude > 0)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(refCamera.HorizontalRotation * velocity),
                apply);
        }
    }

    /// <summary>
    /// �ҋ@
    /// </summary>
    void Idle()
    {
        animator.SetInteger(moveParamHash, (int)PlayerAnimation.Idle);
    }

    /// <summary>
    /// �ړ�
    /// </summary>
    /// <param name="speed"></param>
    /// <param name="animationParam"></param>
    void Transfer(int speed, int animationParam)
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            playerState = PlayerState.Run;
        }

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

        velocity = velocity.normalized * speed * Time.deltaTime;
    }

    /// <summary>
    /// �U��
    /// </summary>
    void Attack()
    {
        animator.SetTrigger(attackParamHash);
    }
    
    /// <summary>
    /// ����
    /// </summary>
    void Tired()
    {
        const float tiredTime = 5.0f;

        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Run"))
        {
            //���葱����Ɣ��āA���鑬�x���ቺ����
            accumulationTimer += Time.deltaTime;
            if (accumulationTimer > tiredTime)
            {
                if (walkSpeed >= runSpeed)
                {
                    return;
                }
                
                runSpeed--;
                accumulationTimer = 0;
            }
        }
        else if(animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            //�U����������ƍU�����x���ቺ����
            accumulateFatigue++;
            if (motionSpeed < 0.6)
            {
                //return;
            }
            else if (accumulateFatigue > 3)
            {
                motionSpeed -= 0.2f;
                accumulateFatigue = 0;
            }
        }
        animator.SetFloat(motionParamHash, motionSpeed);
    }

    /// <summary>
    /// �̗͉񕜂�����
    /// </summary>
    void Recovery()
    {
        int strengthRecovery = 4;
        const float strengthRecoveryTime = 8.0f;
        const float attackRecoveryTime = 5.0f;
        float attackRecovery = 1.0f;
        

        //�ቺ�����Ԃ񂾂��̗͉񕜂�����   
        if (strengthRecovery > runSpeed)
        {
            strengthRecoveryTimer += Time.deltaTime;

            if (strengthRecoveryTimer > strengthRecoveryTime)
            {
                runSpeed++;
                strengthRecoveryTimer = 0;
                accumulationTimer = 0;
            } 
        }

        if(attackRecovery > motionSpeed)
        {
            attackRecoveryTimer += Time.deltaTime;

            if(attackRecoveryTimer > attackRecoveryTime)
            {
                motionSpeed += 0.2f;
                attackRecoveryTimer = 0;
            }
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.tag == "EnemyWeapon")
        {
            TakeDamage();
            StartCoroutine(HitStop());
            if (hitPoint <= 0)
            {
                isActive = false;
                animator.SetBool("Die", true);
            }
        }
    }

    IEnumerator HitStop()
    {
        animator.speed = 0f;
        yield return new WaitForSeconds(hitStopTime);
        animator.speed = 1f;
    }

    /// <summary>
    /// �_���[�W���󂯂�
    /// </summary>
    void TakeDamage()
    {
        //�_���[�W���󂯂��Ԃ񂾂��q�b�g�|�C���g�����炷
        if(hitPoint > 0)
        {
            hitPointArray[hitPoint - 1].SetActive(false);
            hitPoint--;
            audioSource.PlayOneShot(damageSound);
        }
    }

    void AttackVoiceSound()
    {
        audioSource.PlayOneShot(attackSound);
    }

    /// <summary>
    /// ��ԑJ�ڂ̕ύX
    /// </summary>
    /// <param name="stateValue"></param>
    void ChangeState(PlayerState stateValue)
    {
        if(stateValue == PlayerState.Idle)
        {
            if (Input.GetMouseButtonDown(0))
            {
                playerState = PlayerState.Attack;
            }
            else if(!Input.GetMouseButtonDown(0) && !animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
            {
                playerState = PlayerState.Walk;
            }
        }
        else if(stateValue == PlayerState.Walk)
        {
            if(!Input.anyKey) 
            {
                playerState = PlayerState.Idle;
            }
            else if(Input.GetMouseButtonDown(0) && !animator.GetCurrentAnimatorStateInfo(0).IsTag("Walk"))
            {
                playerState = PlayerState.Attack;
            }
        }
        else if (stateValue == PlayerState.Run)
        {
            if (!Input.anyKey)
            {
                playerState = PlayerState.Idle;
            }
            else if (Input.GetMouseButtonDown(0) && !animator.GetCurrentAnimatorStateInfo(0).IsTag("Run"))
            {
                playerState = PlayerState.Attack;
            }
        }
        else if(stateValue == PlayerState.Attack)
        {
            playerState = PlayerState.Idle;
        }
    }
}