using Godot;

using RandomDungeons.Nodes.Components;
using RandomDungeons.StateMachines;

namespace RandomDungeons.Nodes.Elements.Enemies
{
    public class ObliviousZombie : BaseEnemy
    {
        [Export] public float MinIdleTime = 1f;
        [Export] public float MaxIdleTime = 2f;
        [Export] public float WanderTime = 3;
        [Export] public float WanderSpeed = 32;

        protected override Node2D Visuals() => GetNode<Node2D>("%Visuals");
        protected override HurtBox Hurtbox() => GetNode<HurtBox>("%HurtBox");
        protected override IState InitialState() => Idle;

        protected override void OnHitWall() => ChangeState(Wander);
        protected override void OnKnockbackFinished() => ChangeState(Wander);

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
            private Vector2 _velocity;

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
                _velocity = Owner.WanderSpeed * dir;
            }

            public override void _PhysicsProcess(float delta)
            {
                Owner.MoveAndSlide(_velocity);

                _timer -= delta;
                if (_timer <= 0)
                    ChangeState(Owner.Idle);
            }
        }
    }
}
