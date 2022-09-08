using Godot;

namespace RandomDungeons.StateMachines
{
    public class StateMachine : Node, IStateMachine
    {
        public IState CurrentState {get; private set;}
        private readonly Node _owner;

        public StateMachine(Node owner)
        {
            _owner = owner;
            owner.AddChild(this);
        }

        public void ChangeState(IState state)
        {
            if (state != null)
            {
                state.StateMachine = this;
                state.Owner = _owner;
            }

            var prevState = CurrentState;
            CurrentState = state;

            prevState?._StateExited();
            CurrentState?._StateEntered();
        }

        public override void _Process(float delta)
        {
            CurrentState?._Process(delta);
        }

        public override void _PhysicsProcess(float delta)
        {
            CurrentState?._PhysicsProcess(delta);
        }
    }
}
