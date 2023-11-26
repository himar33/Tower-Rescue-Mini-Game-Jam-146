using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Jam;

public class EnemyState : Jam.State
{
    public DiscipleStates ID => mID;

    protected EnemyController mEnemy = null;
    protected DiscipleStates mID;
    public EnemyState(Jam.StateBehaviour sM, EnemyController  enemyController) : base(sM)
    {
        mEnemy = enemyController;
    }
    public EnemyState(EnemyController enemyController) : base()
    {
        mEnemy = enemyController;
        _stateBehaviour = mEnemy.StateMachine;
    }
    public override void OnEnter() { base.OnEnter(); }
    public override void OnExit() { base.OnExit(); }
    public override void Update() { base.Update(); }
    public override void FixedUpdate() { base.FixedUpdate(); }
}
