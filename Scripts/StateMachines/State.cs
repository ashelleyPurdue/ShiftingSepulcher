namespace RandomDungeons
{
    public abstract class State<TOwner> : IState
    {
        public StateMachine StateMachine {get; set;}

        object IState.Owner
        {
            get => this.Owner;
            set => this.Owner = (TOwner)value;
        }

        public TOwner Owner {get; set;}

        protected void ChangeState(IState state)
        {
            StateMachine.ChangeState(state);
        }

        public virtual void _Process(float delta) {}
        public virtual void _PhysicsProcess(float delta) {}

        public virtual void _StateEntered() {}
        public virtual void _StateExited() {}
    }
}
