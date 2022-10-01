using Godot;
using RandomDungeons.Nodes.Components;
using RandomDungeons.StateMachines;
using RandomDungeons.Utils;

namespace RandomDungeons.Nodes.Elements.Enemies
{
    public class MiniTilemancer : KinematicBody2D
    {
        [Export] public float TileSpawnRadius = 32 * 2;
        [Export] public float WanderSpeed = 32;
        [Export] public float TileThrowSpeed = 32 * 19;
        [Export] public PackedScene TilePrefab;

        private Node2D _target => GetTree().FindPlayer();


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
