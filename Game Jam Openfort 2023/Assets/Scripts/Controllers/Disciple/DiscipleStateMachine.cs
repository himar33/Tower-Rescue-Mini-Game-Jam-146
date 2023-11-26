using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DiscipleStates
{
    IDLE = 0,
    FOLLOW,
    DIE
}

public class DiscipleStateMachine : Jam.StateBehaviour
{
    public DiscipleStateMachine() : base() { }
    public void Add(DiscipleState state)
    {
        _states.Add((int)state.ID, state);
    }
    public DiscipleState GetState(DiscipleStates key)
    {
        return (DiscipleState)GetState((int)key);
    }
    public void Set(DiscipleStates stateKey)
    {
        Jam.State state = _states[(int)stateKey];
        if (state != null)
        {
            Set(state);
        }
    }
}
