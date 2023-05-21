using System;
using Godot;

namespace ShiftingSepulcher
{
    public abstract class PauseState<TOwner> : State<TOwner>
    {
        public abstract float Duration {get;}
        public abstract IState NextState {get;}

        private float _timer;

        protected float PercentComplete => 1f - (_timer / Duration);

        public override void _StateEntered()
        {
            _timer = Duration;
        }

        public override void _PhysicsProcess(float delta)
        {
            _timer -= delta;

            if (_timer <= 0)
                ChangeState(NextState);
        }
    }
}
