using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [SerializeField] AudioClip attack;
    [SerializeField] AudioClip damage;
    NavMeshAgent agent;
    GameObject player = null;
    Animator animator;
    AudioSource audioSource;
    Dictionary<string, EnemyStateBase.Action> actions = null;
    EnemyStateBase currentState = null;
    int hitPoint = 4;
    /// <summary>
    ///　TargetとEnemyまでの距離
    /// </summary>
    float distance;
    /// <summary>
    /// 索敵範囲
    /// </summary>
    float trackingRange = 4f;

    float hitStopTime = 0.1f;

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
            hitPoint--;
            StartCoroutine(HitStop());
            audioSource.PlayOneShot(damage);
            if(hitPoint > 0)
            {
                animator.SetBool("Damage", true);
            }
            else
            {
                animator.SetBool("Death", true);
            }
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

        if (distance < trackingRange)
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

    IEnumerator HitStop()
    {
        animator.speed = 0;
        yield return new WaitForSeconds(hitStopTime);
        animator.speed = 1;
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
