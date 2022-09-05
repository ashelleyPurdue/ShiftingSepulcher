namespace RandomDungeons.StateMachines
{
    public abstract class State<TOwner> : IState where TOwner : IStateMachine
    {
        IStateMachine IState.Owner
        {
            get => this.Owner;
            set => this.Owner = (TOwner)value;
        }

        public TOwner Owner {get; set;}

        protected void ChangeState(IState state)
        {
            Owner.ChangeState(state);
        }

        public virtual void _Process(float delta) {}
        public virtual void _PhysicsProcess(float delta) {}

        public virtual void _StateEntered() {}
        public virtual void _StateExited() {}
    }
}
