using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] AudioClip attack;
    [SerializeField] AudioClip damage;
    GameObject tpsCamera = null;
    TpsCamera refCamera = null;
    NavMeshAgent agent;
    GameObject player = null;
    Animator animator;
    AudioSource audioSource;
    Dictionary<string, EnemyStateBase.Action> actions = null;
    EnemyStateBase currentState = null;
    int hitPoint = 4;
    int count = 1;
    /// <summary>
    ///　TargetとEnemyまでの距離
    /// </summary>
    float distance = 0;
    [SerializeField] float range = 0.9f;
    /// <summary>
    /// 索敵範囲
    /// </summary>
    [SerializeField] float trackingRange = 4f;
    float hitStopTime = 0.5f;
    float width = 1;
    float duration = 1;

    public Animator Animator
    {
        get { return animator; }
    }

    public GameObject Target
    {
        get { return player; }
    }

    enum GoblinAnimation
    {
        Idle,
        Chase,
        Attack,
    }

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        player = GameObject.Find("Player");
        tpsCamera = GameObject.Find("Camera");
        refCamera = tpsCamera.GetComponent<TpsCamera>();
        actions = new Dictionary<string, EnemyStateBase.Action>();
        actions["Move"] = Move;
        currentState = new EnemyWaitState();
    }

    void Update()
    {
        //StateのUpdate処理をループさせている
        currentState = currentState.Update(this, actions, animator);
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Weapon")
        {
            //hitPoint--;
            audioSource.PlayOneShot(damage);
            if(hitPoint <= 0)
            {
                animator.SetBool("Death", true);
                return;
            }
            Damage(player.transform.forward);
            refCamera.Shake(width, count, duration);
        }
    }

    /// <summary>
    /// Enemyの移動処理
    /// </summary>
    /// <param name="arg"></param>
    void Move(ref EnemyStateBase.ActionArg arg)
    {
        agent.destination = arg.velocity;
    }

    /// <summary>
    /// 索敵範囲の判定を行う
    /// </summary>
    /// <returns></returns>
    public bool TrackingRange()
    {
        distance = Vector3.Distance(player.transform.position, transform.position);

        if (player.transform.position.y > 2)
        {
            return false;
        }
        else if (distance < trackingRange)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 攻撃範囲の判定を行う
    /// </summary>
    /// <param name="distance"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    public bool AttackRange()
    {
        distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance < range)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// NavMeshAgentの自動更新の切り替え
    /// </summary>
    public void UpdatePosition()
    {
        if (currentState.GetState() == EnemyStateBase.State.Wait ||
            animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
        {
            agent.updatePosition = false;
        }
        else if(currentState.GetState() == EnemyStateBase.State.Chase)
        {
            agent.updatePosition = true;
        }
    }

    private void Damage(Vector3 forward)
    {
        Sequence seq = DOTween.Sequence();
        seq.SetDelay(hitStopTime);
                                           // 遅延を待った後、吹っ飛ぶ演出を再生
        Vector3 backPosition = transform.position + forward.normalized * 1f;
        seq.Append(transform.DOMove(backPosition, 0.2f));

        // ダメージモーション再生
        animator.CrossFade("Damage", 0.2f);
    }

    /// <summary>
    /// アニメーション中にbool型のパラメータを切り替える
    /// </summary>
    void DamageEvent()
    {
        animator.SetBool("Damage", false);
    }

    /// <summary>
    /// イベントで音声を流す
    /// </summary>
    void PlayAttackVoice()
    {
        audioSource.PlayOneShot(attack);
    }

    /// <summary>
    /// 倒されたらEnemyを消す
    /// </summary>
    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
