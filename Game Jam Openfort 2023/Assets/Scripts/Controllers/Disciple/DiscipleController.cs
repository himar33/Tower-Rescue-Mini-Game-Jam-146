using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct FlockingData
{
    public Transform Player;
    public float FollowSpeed;
    public float SeparationDistance;
    public float MinDistanceToPlayer;
    public float SeparationForce;
}

public class DiscipleController : MonoBehaviour
{
    public Jam.StateBehaviour StateMachine => mStateMachine;

    [Header("Flocking Settings")]
    public Transform mPlayer;
    public float mFollowSpeed = 2f;
    public float mSeparationDistance = 1f;
    public float mMinDistanceToPlayer = 5f;
    public float mSeparationForce = 10f;

    private Jam.StateBehaviour mStateMachine;
    private FlockingData mFlockData;

    private void Start()
    {
        mFlockData = new FlockingData()
        {
            Player = mPlayer,
            FollowSpeed = mFollowSpeed,
            SeparationDistance = mSeparationDistance,
            MinDistanceToPlayer = mMinDistanceToPlayer,
            SeparationForce = mSeparationForce
        };

        mStateMachine = new DiscipleStateMachine();

        StateMachine.Add((int)DiscipleStates.IDLE, new DiscipleIdleState(this, mFlockData));
        StateMachine.Add((int)DiscipleStates.FOLLOW, new DiscipleFollowState(this, mFlockData));
        StateMachine.Add((int)DiscipleStates.DIE, new DiscipleDieState(this));

        StateMachine.Set(StateMachine.GetState((int)DiscipleStates.FOLLOW));
    }

    private void Update()
    {
        StateMachine.Update();
    }

    private void FixedUpdate()
    {
        StateMachine.FixedUpdate();
    }
}
