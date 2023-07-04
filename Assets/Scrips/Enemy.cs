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
    ///�@Target��Enemy�܂ł̋���
    /// </summary>
    float distance;
    /// <summary>
    /// ���G�͈�
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
        //State��Update���������[�v�����Ă���
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
    /// Enemy�̈ړ�����
    /// </summary>
    /// <param name="arg"></param>
    void Move(ref EnemyStateBase.ActionArg arg)
    {
        agent.destination = arg.velocity;
    }

    /// <summary>
    /// ���G�͈͂̔�����s��
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
    /// NavMeshAgent�̎����X�V�̐؂�ւ�
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
    /// �A�j���[�V��������bool�^�̃p�����[�^��؂�ւ���
    /// </summary>
    void DamageEvent()
    {
        animator.SetBool("Damage", false);
    }

    /// <summary>
    /// �C�x���g�ŉ����𗬂�
    /// </summary>
    void PlayAttackVoice()
    {
        audioSource.PlayOneShot(attack);
    }

    /// <summary>
    /// �|���ꂽ��Enemy������
    /// </summary>
    public void DestroySelf()
    {
        Destroy(this.gameObject);
    }
}
