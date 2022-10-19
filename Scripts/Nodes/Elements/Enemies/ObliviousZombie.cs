using Godot;

using RandomDungeons.StateMachines;
using RandomDungeons.StateMachines.CommonStates;
using RandomDungeons.Utils;

namespace RandomDungeons.Nodes.Elements.Enemies
{
    public class ObliviousZombie : Node
    {
        [Export] public float MinIdleTime = 1f;
        [Export] public float MaxIdleTime = 2f;
        [Export] public float WanderTime = 1;
        [Export] public float WanderSpeed = 32 * 3;

        private EnemyBody _body => this.FindAncestor<EnemyBody>();
        private StateMachine _sm;

        public override void _Ready()
        {
            _sm = new StateMachine(this);
            _sm.ChangeState(Idle);
        }

        public void OnDead()
        {
            _body.WalkVelocity = Vector2.Zero;

            var deathAnim = new DeathAnimationState();
            deathAnim.AnimationTarget = GetNode<Node2D>("%Visuals");
            deathAnim.AnimationEnded += _body.QueueFree;

            _sm.ChangeState(deathAnim);
        }

        public void OnHitWall()
        {
            if (_body.Health > 0)
                _sm.ChangeState(Wander);
        }

        private readonly IdleState Idle = new IdleState();
        private class IdleState : State<ObliviousZombie>
        {
            private float _timer;

            public override void _StateEntered()
            {
                _timer = (float)GD.RandRange(
                    Owner.MinIdleTime,
                    Owner.MaxIdleTime
                );
            }

            public override void _PhysicsProcess(float delta)
            {
                _timer -= delta;

                if (_timer <= 0)
                    ChangeState(Owner.Wander);
            }
        }

        private readonly WanderState Wander = new WanderState();
        private class WanderState : State<ObliviousZombie>
        {
            private float _timer;

            public override void _StateEntered()
            {
                _timer = Owner.WanderTime;

                // Choose a random direction
                float angle = (float)GD.RandRange(0, 360);
                var dir = new Vector2(
                    Mathf.Cos(Mathf.Deg2Rad(angle)),
                    Mathf.Sin(Mathf.Deg2Rad(angle))
                );

                // Start walking in that direction, at the appropriate speed
                Owner._body.WalkVelocity = Owner.WanderSpeed * dir;
            }

            public override void _PhysicsProcess(float delta)
            {
                _timer -= delta;

                if (_timer <= 0)
                    ChangeState(Owner.Idle);
            }
        }
    }
}
