using Godot;

namespace RandomDungeons
{
    public class Zombie : Node
    {
        [Export] public float MinIdleTime = 1f;
        [Export] public float MaxIdleTime = 2f;
        [Export] public float WanderSpeed = 32 * 3;

        private EnemyBody _body => this.FindAncestor<EnemyBody>();
        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%AnimationPlayer");

        private Player _targetPlayer;

        private StateMachine _sm;

        public Zombie()
        {
            _sm = new StateMachine(this);
        }

        public void OnDead()
        {
            _sm.ChangeState(Dead);
        }

        public void OnRespawning()
        {
            _sm.ChangeState(Searching);
        }

        public void OnVisionCircleBodyEntered(object body)
        {
            if (body is Player p && _sm.CurrentState == Searching)
            {
                _targetPlayer = p;
                _sm.ChangeState(Chasing);
            }
        }

        public void StartWandering()
        {
            // Choose a random direction to walk in
            float angle = Mathf.Deg2Rad(GD.Randf() * 360);
            _body.WalkVelocity = WanderSpeed * new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            );

            _animator.PlaybackSpeed = 1;
        }

        public void StartIdling()
        {
            _body.WalkVelocity = Vector2.Zero;

            float idleDuration = (float)GD.RandRange(
                MinIdleTime,
                MaxIdleTime
            );
            _animator.PlaybackSpeed = 1f / idleDuration;
        }

        private State<Zombie> Searching = new SearchingState();
        private class SearchingState : State<Zombie>
        {
            public override void _StateEntered()
            {
                Owner._animator.Play("RESET");
                Owner._animator.Advance(0);
                Owner._animator.Play("Search");
            }

            public override void _StateExited()
            {
                Owner._animator.PlaybackSpeed = 1;
            }
        }

        private State<Zombie> Chasing = new ChasingState();
        private class ChasingState : State<Zombie>
        {
            public override void _StateEntered()
            {
                Owner._animator.Play("RESET");
                Owner._animator.Advance(0);

                // TODO: Play a special chasing animation, once this guy has
                // actual graphics.
                Owner._animator.Stop();
            }

            public override void _PhysicsProcess(float delta)
            {
                var dir = Owner
                    ._body
                    .GlobalPosition
                    .DirectionTo(Owner._targetPlayer.GlobalPosition);

                Owner._body.WalkVelocity = dir * Owner.WanderSpeed;
            }
        }

        private State<Zombie> Dead = new DeadState();
        private class DeadState : State<Zombie>
        {
            public override void _StateEntered()
            {
                Owner._animator.Play("RESET");
                Owner._animator.Advance(0);
                Owner._animator.Play("Death");

                Owner._body.WalkVelocity = Vector2.Zero;
            }
        }
    }
}
