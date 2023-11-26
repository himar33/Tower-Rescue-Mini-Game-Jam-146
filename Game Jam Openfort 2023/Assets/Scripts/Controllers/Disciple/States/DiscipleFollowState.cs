using Jam;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiscipleFollowState : DiscipleState
{
    public DiscipleFollowState(DiscipleController discipleSM) : base(discipleSM)
    {
    }

    public DiscipleFollowState(StateBehaviour sM, DiscipleController discipleSM) : base(sM, discipleSM)
    {
    }
}
