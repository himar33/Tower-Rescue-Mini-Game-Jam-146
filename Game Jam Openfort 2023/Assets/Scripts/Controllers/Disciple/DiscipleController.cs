using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscipleController : MonoBehaviour
{
    public Jam.StateBehaviour StateMachine;

    private void Start()
    {
        StateMachine = new DiscipleStateMachine();

        StateMachine.Add((int)DiscipleStates.IDLE, new DiscipleIdleState(this));
        StateMachine.Add((int)DiscipleStates.FOLLOW, new DiscipleFollowState(this));
        StateMachine.Add((int)DiscipleStates.DIE, new DiscipleDieState(this));
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
