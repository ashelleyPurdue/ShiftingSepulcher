using Godot;
using RandomDungeons.Nodes.Components;
using RandomDungeons.StateMachines;
using RandomDungeons.Utils;

namespace RandomDungeons.Nodes.Elements.Enemies
{
    public class MiniTilemancer : BaseEnemy
    {
        [Export] public float TileSpawnRadius = 32 * 2;
        [Export] public float TileSpinUpTime = 1;
        [Export] public float TileHoldTime = 2;
        [Export] public float TileSpawnCooldownTime = 1;
        [Export] public float TileFlySpeed = 600;

        [Export] public float WanderSpeed = 32;
        [Export] public float WanderTime = 1;
        [Export] public float WanderPauseTime = 1;

        [Export] public PackedScene TilePrefab;

        private Node2D _target => GetTree().FindPlayer();

        protected override HurtBox Hurtbox() => GetNode<HurtBox>("%HurtBox");
        protected override Node2D Visuals() => GetNode<Node2D>("%Visuals");
        protected override IState InitialState() => SummoningTile;

        private readonly IState SummoningTile = new SummoningTileState();
        private class SummoningTileState : State<MiniTilemancer>
        {
            private float _timer;

            public override void _StateEntered()
            {
                _timer =
                    Owner.TileSpinUpTime +
                    Owner.TileHoldTime +
                    Owner.TileSpawnCooldownTime;

                Owner.SummonTile();
            }

            public override void _PhysicsProcess(float delta)
            {
                _timer -= delta;

                if (_timer <= 0)
                    ChangeState(Owner.Wandering);
            }
        }

        private readonly IState Wandering = new WanderingState();
        private class WanderingState : State<MiniTilemancer>
        {
            private float _timer;
            private Vector2 _velocity;

            public override void _StateEntered()
            {
                _timer = Owner.WanderTime;

                float angle = Mathf.Deg2Rad(GD.Randf() * 360);
                _velocity = Owner.WanderSpeed * new Vector2(
                    Mathf.Cos(angle),
                    Mathf.Sin(angle)
                );
            }

            public override void _PhysicsProcess(float delta)
            {
                Owner.MoveAndSlide(_velocity);

                _timer -= delta;

                if (_timer <= 0)
                    ChangeState(Owner.Pausing);
            }
        }

        private readonly IState Pausing = new PausingState();
        private class PausingState : State<MiniTilemancer>
        {
            private float _timer;

            public override void _StateEntered()
            {
                _timer = Owner.WanderPauseTime;
            }

            public override void _PhysicsProcess(float delta)
            {
                _timer -= delta;

                if (_timer <= 0)
                    ChangeState(Owner.SummoningTile);
            }
        }

        private void SummonTile()
        {
            var tile = TilePrefab.Instance<AnimatedTile>();
            GetParent().AddChild(tile);

            tile.GlobalPosition = RandomTileSpawnPos();
            tile.Target = _target;
            tile.SpinUpTime = TileSpinUpTime;
            tile.HoldTime = TileHoldTime;
            tile.FlySpeed = TileFlySpeed;
        }

        private Vector2 RandomTileSpawnPos()
        {
            float angleDeg = Mathf.Rad2Deg(GetAngleTo(_target.GlobalPosition));
            angleDeg += (GD.Randf() * 180) - 90;

            float angle = Mathf.Deg2Rad(angleDeg);
            var dir = new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            );

            return GlobalPosition + (dir * TileSpawnRadius);
        }
    }
}
