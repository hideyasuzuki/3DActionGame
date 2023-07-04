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
    /// Playerの速度
    /// </summary>
    Vector3 velocity = Vector3.zero;
    /// <summary>
    /// 走る速さ
    /// </summary>
    public int runSpeed = 4;
    /// <summary>
    /// 歩く速さ
    /// </summary>
    int walkSpeed = 2;
    /// <summary>
    /// アニメーションのParametersのid
    /// </summary>
    int moveParamHash = Animator.StringToHash("Move");
    /// <summary>
    /// アニメーションのParametersのid
    /// </summary>
    int attackParamHash = Animator.StringToHash("Attack");
    /// <summary>
    /// アニメーションのParametersのid
    /// </summary>
    int motionParamHash = Animator.StringToHash("MotionSpeed");
    /// <summary>
    /// 疲労が蓄積する
    /// </summary>
    int accumulateFatigue = 0;
    int hitPoint = 3;
    float strengthRecoveryTimer = 0.0f;
    float attackRecoveryTimer = 0.0f;
    float accumulationTimer = 0f;
    /// <summary>
    /// 振り向きの速さ
    /// </summary>
    float apply = 0.05f;
    /// <summary>
    /// アニメーション再生の速さ
    /// </summary>
    float motionSpeed = 1.0f;
    float hitStopTime = 0.1f;
    /// <summary>
    /// Playerのアクティブ状態の変更
    /// </summary>
    bool isActive = true;

    public bool IsActive
    {
        get { return isActive; }
    }
    
    /// <summary>
    /// Playerのアニメーションid
    /// </summary>
    enum PlayerAnimation
    {
        Idle,
        Walk,
        Run,
        Attack,
    }

    /// <summary>
    /// Playerの状態遷移id
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
        //前フレームの速度の値が入っているため値を初期化する
        velocity = Vector3.zero;
        //武器が変更されたらアニメーションを変更する
        if (changeWeapon.IsChange && animator.runtimeAnimatorController != runtimeAnimatorController[0])
        {
            animator.runtimeAnimatorController = runtimeAnimatorController[0];
            animator.SetFloat(motionParamHash, motionSpeed);
        }

        //非アクティブの時Playerが動かないようにする
        if (isActive)
        {
            Controller();
        }

        float gravity = 10.0f;
        //velocity.y -= gravity * Time.deltaTime;
        //カメラの向きに移動させる
        characterController.Move(refCamera.HorizontalRotation * velocity);
    }

    void Controller()
    {
        //Playerの状態遷移
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

        //移動しているならPlayerが移動している方向を向く
        if (velocity.magnitude > 0)
        {
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                Quaternion.LookRotation(refCamera.HorizontalRotation * velocity),
                apply);
        }
    }

    /// <summary>
    /// 待機
    /// </summary>
    void Idle()
    {
        animator.SetInteger(moveParamHash, (int)PlayerAnimation.Idle);
    }

    /// <summary>
    /// 移動
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
    /// 攻撃
    /// </summary>
    void Attack()
    {
        animator.SetTrigger(attackParamHash);
    }
    
    /// <summary>
    /// 疲れる
    /// </summary>
    void Tired()
    {
        const float tiredTime = 5.0f;

        if (animator.GetCurrentAnimatorStateInfo(0).IsTag("Run"))
        {
            //走り続けると疲れて、走る速度が低下する
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
            //攻撃し続けると攻撃速度が低下する
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
    /// 体力回復させる
    /// </summary>
    void Recovery()
    {
        int strengthRecovery = 4;
        const float strengthRecoveryTime = 8.0f;
        const float attackRecoveryTime = 5.0f;
        float attackRecovery = 1.0f;
        

        //低下したぶんだけ体力回復させる   
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
    /// ダメージを受ける
    /// </summary>
    void TakeDamage()
    {
        //ダメージを受けたぶんだけヒットポイントを減らす
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
    /// 状態遷移の変更
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