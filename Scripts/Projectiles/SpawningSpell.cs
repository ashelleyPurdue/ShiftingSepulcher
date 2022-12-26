using Godot;

namespace RandomDungeons
{
    public class SpawningSpell : Node2D
    {
        [Export] public float Speed = 32 * 4;
        [Export] public Vector2 TargetPosGlobal;

        public Node2D NodeToSpawn;

        public override void _PhysicsProcess(float delta)
        {
            GlobalPosition = GlobalPosition.MoveToward(TargetPosGlobal, Speed * delta);

            if (GlobalPosition == TargetPosGlobal)
            {
                this.GetRoom().AddChild(NodeToSpawn);
                NodeToSpawn.GlobalPosition = TargetPosGlobal;
                QueueFree();
            }
        }
    }
}
