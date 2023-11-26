using Jam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

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
        Vector3 lastDistance = mEnemy.Target.position - mEnemy.transform.position;

        Vector3 distanceToTarget = lastDistance;
        foreach (var item in mEnemy.Player.Disciples)
        {
            Vector3 discipleTarget = item.transform.position - mEnemy.transform.position;
            if (discipleTarget.magnitude < mEnemy.DetectionRadius)
            {
                if (discipleTarget.magnitude < distanceToTarget.magnitude)
                {
                    mEnemy.Target = item.transform;
                    distanceToTarget = discipleTarget;
                }
            }
        }
        Vector3 playerDistance = mEnemy.Player.transform.position - mEnemy.transform.position;
        if (playerDistance.magnitude < distanceToTarget.magnitude)
        {
            mEnemy.Target = mEnemy.Player.transform;
            distanceToTarget = playerDistance;
        }

        if (distanceToTarget.magnitude < mEnemy.DistanceToAttack)
        {
            mEnemy.StateMachine.Set(mEnemy.StateMachine.GetState((int)EnemyStates.ATTACK));
            return;
        }
        if (distanceToTarget.magnitude > mEnemy.DetectionRadius)
        {
            mEnemy.StateMachine.Set(mEnemy.StateMachine.GetState((int)EnemyStates.IDLE));
            return;
        }


        Vector3 direction = new Vector3(distanceToTarget.x, distanceToTarget.y);

        direction = Quaternion.Euler(30, 0, 0) * direction;

        // Move towards the player with a natural delay
        mEnemy.transform.position += direction.normalized * mEnemy.ChaseSpeed * Time.deltaTime;
    }
}
