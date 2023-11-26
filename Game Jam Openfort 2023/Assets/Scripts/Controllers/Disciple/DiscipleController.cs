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
    public float TimeStep;
}

public class DiscipleController : MonoBehaviour
{
    public Jam.StateBehaviour StateMachine => mStateMachine;
    public Animator Anim => mAnim;
    public Transform PlayerTransform
    {
        get => mPlayer;
        set => mPlayer = value; 
    }
    public bool IsRescued
    {
        get => mIsRescued;
        set => mIsRescued = value;
    }

    [Header("Flocking Settings")]
    [SerializeField] private Transform mPlayer;
    [SerializeField] private float mFollowSpeed = 2f;
    [SerializeField] private float mSeparationDistance = 1f;
    [SerializeField] private float mMinDistanceToPlayer = 5f;
    [SerializeField] private float mSeparationForce = 10f;
    [SerializeField] private float mTimeStep = 0.1f;

    private Jam.StateBehaviour mStateMachine;
    private FlockingData mFlockData;
    private Animator mAnim;
    private bool mIsRescued = false;

    private void Awake()
    {
        mPlayer = GameObject.FindGameObjectWithTag("Player").transform;
        mAnim = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        mFlockData = new FlockingData()
        {
            Player = mPlayer,
            FollowSpeed = mFollowSpeed,
            SeparationDistance = mSeparationDistance,
            MinDistanceToPlayer = mMinDistanceToPlayer,
            SeparationForce = mSeparationForce,
            TimeStep = mTimeStep
        };

        mStateMachine = new DiscipleStateMachine();

        StateMachine.Add((int)DiscipleStates.IDLE, new DiscipleIdleState(this, mFlockData));
        StateMachine.Add((int)DiscipleStates.FOLLOW, new DiscipleFollowState(this, mFlockData));
        StateMachine.Add((int)DiscipleStates.DIE, new DiscipleDieState(this));

        StateMachine.Set(StateMachine.GetState((int)DiscipleStates.IDLE));
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
