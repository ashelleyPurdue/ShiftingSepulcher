using Godot;

namespace ShiftingSepulcher
{
    public class Chompweed : Node2D
    {
        [Export] public float TrackingTargetDuration = 1;
        [Export] public float LungeDuration = 0.2f;
        [Export] public float PauseAfterLungeDuration = 0.5f;
        [Export] public float RecoverDuration = 0.5f;

        [Export] public float LungeDistance = 4 * 32;

        private Area2D _aggroCircle => GetNode<Area2D>("%AggroCircle");
        private Area2D _hurtBox => GetNode<Area2D>("%HurtBox");
        private Area2D _hitBox => GetNode<Area2D>("%HitBox");
        private Node2D _head => GetNode<Node2D>("%Head");

        private StateMachine _sm;
        private Node2D _aggroTarget;

        public override void _Ready()
        {
            _sm = new StateMachine(this);
        }

        public void OnRespawning()
        {
            _sm.ChangeState(Idle);
        }

        public void OnDead()
        {
            _sm.ChangeState(Dead);
        }

        private Node2D SearchForAggroTarget()
        {
            // TODO: Find enemies, too, not just the player
            foreach (var body in _aggroCircle.GetOverlappingBodies())
            {
                if (body is Player p)
                    return p;
            }

            return null;
        }

        private readonly IState Idle = new IdleState();
        private class IdleState : State<Chompweed>
        {
            public override void _StateEntered()
            {
                Owner._head.Position = Vector2.Zero;
            }

            public override void _PhysicsProcess(float delta)
            {
                Owner._aggroTarget = Owner.SearchForAggroTarget();

                if (IsInstanceValid(Owner._aggroTarget))
                {
                    ChangeState(Owner.TrackingTarget);
                }
            }
        }

        private readonly IState TrackingTarget = new TrackingTargetState();
        private class TrackingTargetState : State<Chompweed>
        {
            private float _timer;

            public override void _StateEntered()
            {
                _timer = 0;
            }

            public override void _PhysicsProcess(float delta)
            {
                // TODO: Rotate towards the target
                _timer += delta;

                if (_timer >= Owner.TrackingTargetDuration)
                    ChangeState(Owner.Lunging);
            }
        }

        private readonly IState Lunging = new LungingState();
        private class LungingState : State<Chompweed>
        {
            private float _timer;
            private Vector2 _lungeDir;

            public override void _StateEntered()
            {
                _timer = 0;

                Vector2 targetPos = Owner._aggroTarget.GlobalPosition;
                _lungeDir = Owner.ToLocal(targetPos).Normalized();
            }

            public override void _PhysicsProcess(float delta)
            {
                _timer += delta;

                Vector2 startPos = Vector2.Zero;
                Vector2 endPos = _lungeDir * Owner.LungeDistance;

                float t = _timer / Owner.LungeDuration;
                Owner._head.Position = startPos.LinearInterpolate(endPos, t);

                if (_timer >= Owner.LungeDuration)
                {
                    Owner._head.Position = endPos;
                    ChangeState(Owner.PausingAfterLunge);
                }
            }
        }

        private readonly IState PausingAfterLunge = new PausingAfterLungeState();
        private class PausingAfterLungeState : State<Chompweed>
        {
            private float _timer;

            public override void _StateEntered()
            {
                _timer = Owner.PauseAfterLungeDuration;
            }

            public override void _PhysicsProcess(float delta)
            {
                _timer -= delta;

                if (_timer <= 0)
                    ChangeState(Owner.Recovering);
            }
        }

        private readonly IState Recovering = new RecoveringState();
        private class RecoveringState : State<Chompweed>
        {
            private float _timer;
            private Vector2 _startPos;

            public override void _StateEntered()
            {
                _timer = 0;
                _startPos = Owner._head.Position;
            }

            public override void _PhysicsProcess(float delta)
            {
                _timer += delta;

                Vector2 endPos = Vector2.Zero;
                float t = _timer / Owner.RecoverDuration;
                Owner._head.Position = _startPos.LinearInterpolate(endPos, t);

                if (_timer >= Owner.RecoverDuration)
                {
                    Owner._head.Position = endPos;
                    ChangeState(Owner.Idle);
                }
            }
        }

        private readonly IState Dead = new DeadState();
        private class DeadState : State<Chompweed>
        {
            public override void _StateEntered()
            {
                // TODO: Play a death animation
                SetEnabled(false);
            }

            public override void _StateExited()
            {
                SetEnabled(true);
            }

            private void SetEnabled(bool enabled)
            {
                Owner._hurtBox.Monitoring = enabled;
                Owner._hurtBox.Monitorable = enabled;

                Owner._hitBox.Monitoring = enabled;
                Owner._hitBox.Monitorable = enabled;

                Owner._head.Visible = enabled;
            }
        }
    }
}
