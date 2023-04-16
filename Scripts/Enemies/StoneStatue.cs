using Godot;

namespace ShiftingSepulcher
{
    public class StoneStatue : KinematicBody2D
    {
        [Export] public float HopDistance = 32;
        [Export] public float HopDuration = 0.2f;
        [Export] public float TimeBetweenHops = 0.4f;
        [Export] public float WakeUpTime = 1;

        private readonly StateMachine _sm;

        private CollisionShape2D _bodyShape => GetNode<CollisionShape2D>("%KinematicBodyShape");
        private HitBox _hitBox => GetNode<HitBox>("%HitBox");
        private HurtBox _hurtBox => GetNode<HurtBox>("%HurtBox");
        private Area2D _aggroCircle => GetNode<Area2D>("%AggroCircle");

        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%AnimationPlayer");
        private Node2D _visuals => GetNode<Node2D>("%StoneStatueModel");

        private KnockbackableVelocityComponent _velocity;
        private Node2D _aggroTarget;
        private Vector2 _lastAggroTargetPos;

        public StoneStatue()
        {
            _sm = new StateMachine(this);
        }

        public override void _Ready()
        {
            _velocity = this.GetComponent<KnockbackableVelocityComponent>();
            _sm.ChangeState(Idle);
        }

        public override void _PhysicsProcess(float delta)
        {
            if (IsInstanceValid(_aggroTarget))
                _lastAggroTargetPos = _aggroTarget.GlobalPosition;
        }

        public void OnRespawning()
        {
            _sm.ChangeState(Idle);
        }

        public void OnDead()
        {
            _sm.ChangeState(Dead);
        }

        private void EnableCollision(bool enabled)
        {
            _bodyShape.SetDeferred("disabled", !enabled);
            _hitBox.Enabled = enabled;
            _hurtBox.Enabled = enabled;
        }

        private bool TryAcquireAggroTarget()
        {
            foreach (var body in _aggroCircle.GetOverlappingBodies())
            {
                if (body is Player p)
                {
                    _aggroTarget = p;
                    _lastAggroTargetPos = _aggroTarget.GlobalPosition;
                    return true;
                }
            }

            _aggroTarget = null;
            return false;
        }

        private readonly IState Idle = new IdleState();
        private class IdleState : State<StoneStatue>
        {
            public override void _PhysicsProcess(float delta)
            {
                if (Owner.TryAcquireAggroTarget())
                    ChangeState(Owner.WakingUp);
            }
        }

        private readonly IState WakingUp = new WakingUpState();
        private class WakingUpState : State<StoneStatue>
        {
            private float _timer;

            public override void _StateEntered()
            {
                _timer = Owner.WakeUpTime;
                Owner._animator.ResetAndPlay(
                    "WakeUp",
                    customSpeed: 1f / Owner.WakeUpTime
                );
            }

            public override void _PhysicsProcess(float delta)
            {
                _timer -= delta;

                if (_timer <= 0)
                    ChangeState(Owner.Hopping);
            }
        }

        private readonly IState PausingBetweenHops = new PausingBetweenHopsState();
        private class PausingBetweenHopsState : State<StoneStatue>
        {
            private float _timer;

            public override void _StateEntered()
            {
                _timer = Owner.TimeBetweenHops;
            }

            public override void _PhysicsProcess(float delta)
            {
                _timer -= delta;

                if (_timer <= 0)
                    ChangeState(Owner.Hopping);
            }
        }

        private readonly IState Hopping = new HoppingState();
        private class HoppingState : State<StoneStatue>
        {
            private float _timer;
            private float _startRot;
            private float _endRot;

            private float _rot
            {
                get => Owner._visuals.Rotation;
                set => Owner._visuals.Rotation = value;
            }

            public override void _StateEntered()
            {
                _timer = Owner.HopDuration;
                Owner._animator.ResetAndPlay("Hop", customSpeed: 1f / Owner.HopDuration);

                float speed = Owner.HopDistance / Owner.HopDuration;
                Vector2 targetPos = Owner._lastAggroTargetPos;
                Vector2 dir = Owner.GlobalPosition.DirectionTo(targetPos);
                Owner._velocity.WalkVelocity = speed * dir;

                _startRot = _rot;
                _endRot = dir.Angle() - Mathf.Deg2Rad(90);
            }

            public override void _PhysicsProcess(float delta)
            {
                _timer -= delta;
                float t = 1 - (_timer / Owner.HopDuration);
                _rot = Mathf.LerpAngle(_startRot, _endRot, t);

                if (_timer <= 0)
                    ChangeState(Owner.PausingBetweenHops);
            }

            public override void _StateExited()
            {
                Owner._velocity.WalkVelocity = Vector2.Zero;
                _rot = _endRot;
            }
        }

        private readonly IState Dead = new DeadState();
        private class DeadState : State<StoneStatue>
        {
            public override void _StateEntered()
            {
                Owner.EnableCollision(false);
                Owner._aggroTarget = null;
                Owner._animator.ResetAndPlay("Death");
            }

            public override void _StateExited()
            {
                Owner.EnableCollision(true);
            }
        }
    }
}
