using Godot;
using RandomDungeons.Nodes.Components;
using RandomDungeons.StateMachines;

namespace RandomDungeons.Nodes.Elements.Enemies
{
    public class MiniTilemancer : BaseEnemy
    {
        [Export] public float TileSpawnRadius = 32 * 2;
        [Export] public float TileSpinUpTime = 1;
        [Export] public float TileHoldTime = 2;
        [Export] public float TileFlySpeed = 600;

        [Export] public PackedScene TilePrefab;

        [Export] public NodePath TargetPath;    // TODO: Find the target some other way
        private Node2D _target => GetNode<Node2D>(TargetPath);

        protected override HurtBox Hurtbox() => GetNode<HurtBox>("%HurtBox");
        protected override Node2D Visuals() => GetNode<Node2D>("%Visuals");
        protected override IState InitialState() => Idle;

        public override void _Ready()
        {
            base._Ready();
            SummonTile();
        }

        private readonly IState Idle = new IdleState();
        private class IdleState : State<MiniTilemancer>
        {
        }

        private void SummonTile()
        {
            var tile = TilePrefab.Instance<AnimatedTile>();
            AddChild(tile);

            tile.GlobalPosition = RandomTileSpawnPos();
            tile.Target = _target;
        }

        private Vector2 RandomTileSpawnPos()
        {
            float angle = Mathf.Deg2Rad(GD.Randf() * 360);
            var dir = new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            );

            return GlobalPosition + (dir * TileSpawnRadius);
        }
    }
}
