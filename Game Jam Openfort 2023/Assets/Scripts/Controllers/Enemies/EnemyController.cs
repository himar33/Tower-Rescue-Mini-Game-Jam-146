using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class EnemyController : MonoBehaviour
{
    public Jam.StateBehaviour StateMachine => mStateMachine;
    public Animator Anim => mAnim;
    public Transform Target
    {
        get => mTarget;
        set => mTarget = value;
    }
    public PlayerController Player => mPlayerController;
    public float DetectionRadius => mDetectionRadius;
    public float ChaseSpeed => mChaseSpeed;
    public float DistanceToAttack => mDistanceToAttack;

    [Header("Enemy Settings")]
    [SerializeField] private float mChaseSpeed;
    [SerializeField] private Transform mTarget;
    [SerializeField] private float mDetectionRadius = 5f;
    [SerializeField] private float mDistanceToAttack = 1f;

    private Jam.StateBehaviour mStateMachine;
    private Animator mAnim;
    private PlayerController mPlayerController;

    private void Awake()
    {
        mAnim = GetComponent<Animator>();
        mPlayerController = FindObjectOfType<PlayerController>();
    }

    private void Start()
    {
        mStateMachine = new EnemyStateMachine();

        StateMachine.Add((int)EnemyStates.IDLE, new EnemyIdle(this));
        StateMachine.Add((int)EnemyStates.CHASE, new EnemyChase(this));
        StateMachine.Add((int)EnemyStates.ATTACK, new EnemyAttack(this));

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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, mDetectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, mDistanceToAttack);
    }

    public void AttackFinished()
    {
        Anim.ResetTrigger("CanAttack");

        Vector3 distanceToTarget = Target.position - transform.position;
        if (distanceToTarget.magnitude > DetectionRadius)
        {
            StateMachine.Set(StateMachine.GetState((int)EnemyStates.IDLE));
        }
        else if (distanceToTarget.magnitude < DistanceToAttack)
        {
            StateMachine.Set(StateMachine.GetState((int)EnemyStates.ATTACK));
        }
        else
        {
            StateMachine.Set(StateMachine.GetState((int)EnemyStates.CHASE));
        }
    }
}
