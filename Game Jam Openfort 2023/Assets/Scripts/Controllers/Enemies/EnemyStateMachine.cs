using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyStates
{
    IDLE = 0,
    CHASE,
    ATTACK
}

public class EnemyStateMachine : Jam.StateBehaviour
{
    public EnemyStateMachine() : base() { }
    public void Add(EnemyState state)
    {
        _states.Add((int)state.ID, state);
    }
    public EnemyState GetState(EnemyStates key)
    {
        return (EnemyState)GetState((int)key);
    }
    public void Set(EnemyStates stateKey)
    {
        Jam.State state = _states[(int)stateKey];
        if (state != null)
        {
            Set(state);
        }
    }
}
