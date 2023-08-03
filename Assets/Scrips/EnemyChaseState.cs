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
        
        //UŒ‚‚Æõ“G‚Ì”ÍˆÍ‚Ì”»’è‚ğ‚µ‚Ä‚©‚çChase‚Ìˆ—‚ğÀs‚·‚é
        if(enemy.AttackRange())
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
}
