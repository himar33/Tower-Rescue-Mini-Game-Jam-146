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

    public override void Update()
    {
        Vector3 directionToPlayer = mData.Player.position - mDiscipleTransform.position;
        Vector3 direction = new Vector3(directionToPlayer.x, directionToPlayer.y);

        if (directionToPlayer.magnitude > mData.MinDistanceToPlayer)
        {
            mDisciple.StateMachine.Set(mDisciple.StateMachine.GetState((int)DiscipleStates.FOLLOW));
        }
    }
}
