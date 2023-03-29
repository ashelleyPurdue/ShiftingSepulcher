using Godot;

namespace ShiftingSepulcher
{
    public class StoneStatue : BaseComponent<KinematicBody2D>
    {
        [Export] public float HopDistance = 32;
        [Export] public float HopDuration = 0.1f;
        [Export] public float TimeBetweenHops = 1;

        private readonly StateMachine _sm;

        private CollisionShape2D _bodyShape => GetNode<CollisionShape2D>("%KinematicBodyShape");
        private HitBox _hitBox => GetNode<HitBox>("%HitBox");
        private HurtBox _hurtBox => GetNode<HurtBox>("%HurtBox");
        private Area2D _aggroCircle => GetNode<Area2D>("%AggroCircle");

        private KnockbackableVelocityComponent _velocity;
        private Node2D _aggroTarget;

        public StoneStatue()
        {
            _sm = new StateMachine(this);
        }

        public override void _EntityReady()
        {
            _velocity = this.GetComponent<KnockbackableVelocityComponent>();
            _sm.ChangeState(Idle);
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

        private Node2D SearchForAggroTarget()
        {
            foreach (var body in _aggroCircle.GetOverlappingBodies())
            {
                if (body is Player p)
                    return p;
            }

            return null;
        }

        private readonly IState Idle = new IdleState();
        private class IdleState : State<StoneStatue>
        {
            public override void _PhysicsProcess(float delta)
            {
                Owner._aggroTarget = Owner.SearchForAggroTarget();

                if (Owner._aggroTarget != null)
                    ChangeState(Owner.PausingBetweenHops);
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

            public override void _StateEntered()
            {
                _timer = Owner.HopDuration;

                float speed = Owner.HopDistance / Owner.HopDuration;
                Vector2 targetPos = Owner._aggroTarget.GlobalPosition;
                Vector2 dir = Owner.Entity.GlobalPosition.DirectionTo(targetPos);
                Owner._velocity.WalkVelocity = speed * dir;
            }

            public override void _PhysicsProcess(float delta)
            {
                _timer -= delta;

                // TODO: Visually rotate towards target

                if (_timer <= 0)
                    ChangeState(Owner.PausingBetweenHops);
            }

            public override void _StateExited()
            {
                Owner._velocity.WalkVelocity = Vector2.Zero;
            }
        }

        private readonly IState Dead = new DeadState();
        private class DeadState : State<StoneStatue>
        {
            public override void _StateEntered()
            {
                Owner.EnableCollision(false);
                Owner.Entity.Visible = false;
                Owner._aggroTarget = null;
            }

            public override void _StateExited()
            {
                Owner.EnableCollision(true);
                Owner.Entity.Visible = true;
            }
        }
    }
}
