using Jam;

public class DiscipleState : Jam.State
{
    public DiscipleStates ID => mID;

    protected DiscipleController mDisciple = null;
    protected DiscipleStates mID;
    public DiscipleState(Jam.StateBehaviour sM, DiscipleController discipleSM) : base(sM)
    {
        mDisciple = discipleSM;
    }
    public DiscipleState(DiscipleController discipleSM) : base()
    {
        mDisciple = discipleSM;
        _stateBehaviour = mDisciple.StateMachine;
    }
    public override void OnEnter() { base.OnEnter(); }
    public override void OnExit() { base.OnExit(); }
    public override void Update() { base.Update(); }
    public override void FixedUpdate() { base.FixedUpdate(); }
}
