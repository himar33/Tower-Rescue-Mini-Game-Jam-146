using Jam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : EnemyState
{
    public EnemyAttack(EnemyController discipleSM) : base(discipleSM)
    {
    }

    public EnemyAttack(StateBehaviour sM, EnemyController discipleSM) : base(sM, discipleSM)
    {
    }

    public override void OnEnter()
    {
        mEnemy.Anim.SetBool("IsMoving", false);

        mEnemy.Anim.SetTrigger("CanAttack");
    }
}
