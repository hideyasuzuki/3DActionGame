using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class EnemyStateBase
{
    public enum State
    {
        Wait,
        Chase,
        Attack,
    }

    public struct ActionArg
    {
        public Vector3 velocity;
    }

    public delegate void Action(ref ActionArg arg);

    public abstract EnemyStateBase Update(Enemy obj, Dictionary<string, EnemyStateBase.Action> actions, Animator animator);

    public abstract State GetState();
}
