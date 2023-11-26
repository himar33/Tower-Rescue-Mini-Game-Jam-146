using Jam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyIdle : EnemyState
{
    public EnemyIdle(EnemyController discipleSM) : base(discipleSM)
    {
    }

    public EnemyIdle(StateBehaviour sM, EnemyController discipleSM) : base(sM, discipleSM)
    {
    }

    public override void OnEnter()
    {
        mEnemy.Anim.SetBool("IsMoving", false);
    }

    public override void Update()
    {
        Vector3 distanceToTarget = mEnemy.Target.position - mEnemy.transform.position;
        if (distanceToTarget.magnitude < mEnemy.DetectionRadius)
        {
            mEnemy.StateMachine.Set(mEnemy.StateMachine.GetState((int)EnemyStates.CHASE));
        }
    }
}
