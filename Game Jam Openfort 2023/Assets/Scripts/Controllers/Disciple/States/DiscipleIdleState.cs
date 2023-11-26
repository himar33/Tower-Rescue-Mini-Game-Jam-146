using Jam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscipleIdleState : DiscipleState
{
    public DiscipleIdleState(DiscipleController discipleSM) : base(discipleSM)
    {
    }

    public DiscipleIdleState(StateBehaviour sM, DiscipleController discipleSM) : base(sM, discipleSM)
    {
    }
}
