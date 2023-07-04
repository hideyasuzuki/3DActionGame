using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyWaitState : EnemyStateBase
{
    public EnemyWaitState()
    {

    }

    public override EnemyStateBase.State GetState()
    {
        return State.Wait;
    }

    public override EnemyStateBase Update(Enemy enemy, Dictionary<string, Action> actions, Animator animator)
    {
        enemy.UpdatePosition();
        animator.SetInteger("Move", (int)State.Wait);
        if(enemy.TrackingRange())
        {
            return new EnemyChaseState();
        }

        return new EnemyWaitState();
    }
}
