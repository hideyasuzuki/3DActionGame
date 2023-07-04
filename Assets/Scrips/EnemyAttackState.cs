using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackState : EnemyStateBase
{
    public EnemyAttackState()
    {

    }

    public override EnemyStateBase.State GetState()
    {
        return State.Attack;
    }

    public override EnemyStateBase Update(Enemy enemy, Dictionary<string, Action> actions, Animator animator)
    {
        EnemyStateBase.Action action = null;
        EnemyStateBase.ActionArg arg;

        if (actions.TryGetValue("Move", out action) == true)
        {
            arg.velocity = enemy.transform.position;
            enemy.UpdatePosition();
            animator.SetInteger("Move", (int)State.Attack);
            action(ref arg);
        }

        if (enemy.TrackingRange())
        {
            return new EnemyChaseState();
        }

        return new EnemyAttackState();
    }
}