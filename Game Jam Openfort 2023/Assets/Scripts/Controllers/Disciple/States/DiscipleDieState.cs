using Jam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscipleDieState : DiscipleState
{
    public DiscipleDieState(DiscipleController discipleSM) : base(discipleSM)
    {
    }

    public DiscipleDieState(StateBehaviour sM, DiscipleController discipleSM) : base(sM, discipleSM)
    {
    }
}
