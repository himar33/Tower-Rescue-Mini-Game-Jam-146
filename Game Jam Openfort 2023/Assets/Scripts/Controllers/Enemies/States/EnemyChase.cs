using Jam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChase : EnemyState
{
    public EnemyChase(EnemyController discipleSM) : base(discipleSM)
    {
    }

    public EnemyChase(StateBehaviour sM, EnemyController discipleSM) : base(sM, discipleSM)
    {
    }

    public override void OnEnter()
    {
        mEnemy.Anim.SetBool("IsMoving", true);
    }

    public override void Update()
    {
        Vector3 distanceToTarget = mEnemy.Target.position - mEnemy.transform.position;
        if (distanceToTarget.magnitude > mEnemy.DetectionRadius)
        {
            mEnemy.StateMachine.Set(mEnemy.StateMachine.GetState((int)EnemyStates.IDLE));
            return;
        }
        if (distanceToTarget.magnitude < mEnemy.DistanceToAttack)
        {
            mEnemy.StateMachine.Set(mEnemy.StateMachine.GetState((int)EnemyStates.ATTACK));
        }

        Vector3 direction = new Vector3(distanceToTarget.x, distanceToTarget.y);

        direction = Quaternion.Euler(30, 0, 0) * direction;

        // Move towards the player with a natural delay
        mEnemy.transform.position += direction.normalized * mEnemy.ChaseSpeed * Time.deltaTime;
    }
}
