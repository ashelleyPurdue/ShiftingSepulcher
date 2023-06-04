using Godot;

namespace ShiftingSepulcher
{
    public class Zombie : KinematicBody2D
    {
        [Export] public float MinIdleTime = 1f;
        [Export] public float MaxIdleTime = 2f;
        [Export] public float WanderTime = 1f;
        [Export] public float WanderSpeed = 32 * 3;

        private Node2D _visuals => GetNode<Node2D>("%Visuals");
        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%AnimationPlayer");

        private Player _targetPlayer;
        private KnockbackableVelocityComponent _body;

        private StateMachine _sm;

        public Zombie()
        {
            _sm = new StateMachine(this);
        }

        public override void _Ready()
        {
            _body = this.GetComponent<KnockbackableVelocityComponent>();

            var enemy = this.GetComponent<EnemyComponent>();
            enemy.Connect(
                signal: nameof(EnemyComponent.Respawning),
                target: this,
                nameof(OnRespawning)
            );
            enemy.Connect(
                signal: nameof(EnemyComponent.Dying),
                target: this,
                nameof(OnDying)
            );
        }

        public override void _Process(float delta)
        {
            if (_body.WalkVelocity.Length() > 0.01f)
                _visuals.Rotation = _body.WalkVelocity.Angle();
        }

        public void OnDying()
        {
            _sm.ChangeState(Dying);
        }

        public void OnRespawning()
        {
            _sm.ChangeState(Idling);
        }

        public void OnVisionCircleBodyEntered(object body)
        {
            if (!(body is Player p))
                return;

            if (_sm.CurrentState == Wandering || _sm.CurrentState == Idling)
            {
                _targetPlayer = p;
                _sm.ChangeState(Chasing);
            }
        }

        private State<Zombie> Idling = new IdlingState();
        private class IdlingState : State<Zombie>
        {
            private float _timer = 0;

            public override void _StateEntered()
            {
                Owner._animator.ResetAndPlay("Idle");
                Owner._body.WalkVelocity = Vector2.Zero;

                _timer = (float)GD.RandRange(
                    Owner.MinIdleTime,
                    Owner.MaxIdleTime
                );
            }

            public override void _PhysicsProcess(float delta)
            {
                _timer -= delta;

                if (_timer <= 0)
                    ChangeState(Owner.Wandering);
            }
        }

        private State<Zombie> Wandering;
        private class WanderingState : State<Zombie>
        {
            private float _timer = 0;

            public override void _StateEntered()
            {
                Owner._animator.ResetAndPlay("Walk");
                _timer = Owner.WanderTime;

                // Choose a random direction to walk in
                float angle = Mathf.Deg2Rad(GD.Randf() * 360);

                Owner._body.WalkVelocity = Owner.WanderSpeed * new Vector2(
                    Mathf.Cos(angle),
                    Mathf.Sin(angle)
                );
            }

            public override void _PhysicsProcess(float delta)
            {
                _timer -= delta;

                if (_timer <= 0)
                    ChangeState(Owner.Idling);
            }
        }

        private State<Zombie> Chasing = new ChasingState();
        private class ChasingState : State<Zombie>
        {
            public override void _StateEntered()
            {
                Owner._animator.ResetAndPlay("Walk");
            }

            public override void _PhysicsProcess(float delta)
            {
                var dir = Owner
                    .GlobalPosition
                    .DirectionTo(Owner._targetPlayer.GlobalPosition);

                Owner._body.WalkVelocity = dir * Owner.WanderSpeed;
            }
        }

        private State<Zombie> Dying = new DyingState();
        private class DyingState : State<Zombie>
        {
            public override void _StateEntered()
            {
                Owner._animator.ResetAndPlay("Death");
                Owner._body.WalkVelocity = Vector2.Zero;
            }

            public override void _Process(float delta)
            {
                if (!Owner._animator.IsPlaying())
                {
                    ChangeState(Owner.Dead);
                    Owner.GetComponent<EnemyComponent>().FireDeathAnimationComplete();
                }
            }
        }

        private State<Zombie> Dead = new DeadState();
        private class DeadState : State<Zombie> {}
    }
}
