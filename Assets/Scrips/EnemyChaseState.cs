using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyChaseState : EnemyStateBase
{
    public EnemyChaseState()
    {

    }

    public override EnemyStateBase.State GetState()
    {
        return State.Chase;
    }

    public override EnemyStateBase Update(Enemy enemy, Dictionary<string, Action> actions, Animator animator)
    {
        EnemyStateBase.Action action = null;
        EnemyStateBase.ActionArg arg;
        float distance = Vector3.Distance(enemy.Target.transform.position, enemy.transform.position);
        float attackRange = 0.9f;
        
        //UŒ‚‚Æõ“G‚Ì”ÍˆÍ‚Ì”»’è‚ğ‚µ‚Ä‚©‚çChase‚Ìˆ—‚ğÀs‚·‚é
        if(AttackRange(distance, attackRange))
        {
            return new EnemyAttackState();
        }     
        else if(!enemy.TrackingRange())
        {
            return new EnemyWaitState();
        }
        else if (actions.TryGetValue("Move", out action) == true)
        {
            enemy.UpdatePosition();
            if(animator.GetCurrentAnimatorStateInfo(0).IsName("Death"))
            {
                return new EnemyWaitState();
            }
            arg.velocity = enemy.Target.transform.position;
            enemy.transform.LookAt(enemy.Target.transform.position);
            animator.SetInteger("Move", (int)State.Chase);
            action(ref arg);
        }

        return new EnemyChaseState();
    }

    /// <summary>
    /// UŒ‚”ÍˆÍ‚Ì”»’è‚ğs‚¤
    /// </summary>
    /// <param name="distance"></param>
    /// <param name="range"></param>
    /// <returns></returns>
    bool AttackRange(float distance, float range)
    {
        if (distance < range)
        {
            return true;
        }
        return false;
    }
}
