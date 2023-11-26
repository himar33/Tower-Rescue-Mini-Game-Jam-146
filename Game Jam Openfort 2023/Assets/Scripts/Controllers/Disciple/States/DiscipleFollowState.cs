using Jam;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DiscipleFollowState : DiscipleState
{
    private FlockingData mData;
    private Transform mDiscipleTransform;
    public DiscipleFollowState(DiscipleController discipleSM, FlockingData mData) : base(discipleSM)
    {
        this.mData = mData;
        mDiscipleTransform = mDisciple.transform;
    }

    public override void OnEnter()
    {
        mDisciple.Anim.SetBool("IsMoving", true);
    }

    public override void Update()
    {
        Vector3 directionToPlayer = mData.Player.position - mDiscipleTransform.position;
        Vector3 direction = new Vector3(directionToPlayer.x, directionToPlayer.y);

        direction = Quaternion.Euler(30, 0, 0) * direction;

        // Move towards the player with a natural delay
        mDiscipleTransform.position += direction.normalized * mData.FollowSpeed * Time.deltaTime;

        // Check if too close to another flock member and separate
        Collider[] neighbors = Physics.OverlapSphere(mDiscipleTransform.position, mData.SeparationDistance);
        foreach (var neighbor in neighbors)
        {
            if (neighbor.transform != mDiscipleTransform)
            {
                Vector3 separationDirection = mDiscipleTransform.position - neighbor.transform.position;
                Vector3 separation = new Vector3(separationDirection.x, separationDirection.y);
                separation = Quaternion.Euler(30, 0, 0) * separation;
                mDiscipleTransform.position += separation.normalized * Time.deltaTime * mData.SeparationForce;
            }
        }

        // Stop when reaching the minimum distance to the player
        if (directionToPlayer.magnitude < mData.MinDistanceToPlayer)
        {
            mDisciple.StateMachine.Set(mDisciple.StateMachine.GetState((int)DiscipleStates.IDLE));
        }

        mDiscipleTransform.rotation = Quaternion.Euler(0f, (mData.Player.position.x >= mDisciple.transform.position.x) ? 180f : 0f, 0f);
    }
}
