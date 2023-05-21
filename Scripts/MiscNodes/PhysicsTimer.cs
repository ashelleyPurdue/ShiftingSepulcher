using Godot;

namespace ShiftingSepulcher
{
    public class PhysicsTimer : Node
    {
        [Signal] public delegate void TimerExpired();

        private float _timer;

        public PhysicsTimer(float duration)
        {
            _timer = duration;
        }

        public override void _PhysicsProcess(float delta)
        {
            _timer -= delta;

            if (_timer <= 0)
            {
                EmitSignal(nameof(TimerExpired));
                QueueFree();
            }
        }
    }
}
