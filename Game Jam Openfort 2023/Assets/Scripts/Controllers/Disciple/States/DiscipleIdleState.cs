using Jam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscipleIdleState : DiscipleState
{
    private FlockingData mData;
    private Transform mDiscipleTransform;
    public DiscipleIdleState(DiscipleController discipleSM, FlockingData mData) : base(discipleSM)
    {
        this.mData = mData;
        mDiscipleTransform = mDisciple.transform;
    }

    public override void OnEnter()
    {
        mDisciple.Anim.SetBool("IsMoving", false);
    }

    public override void Update()
    {
        Vector3 directionToPlayer = mData.Player.position - mDiscipleTransform.position;

        if (directionToPlayer.magnitude > mData.MinDistanceToPlayer && mDisciple.IsRescued)
        {
            mDisciple.StateMachine.Set(mDisciple.StateMachine.GetState((int)DiscipleStates.FOLLOW));
        }

        mDiscipleTransform.rotation = Quaternion.Euler(0f, (mData.Player.position.x >= mDisciple.transform.position.x) ? 180f : 0f, 0f);
    }
}
