using System;
using Godot;

namespace RandomDungeons
{
    public class BombWitch : Node
    {
        [Export] public float TeleportRadius = 6 * 32;

        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%AnimationPlayer");

        public void ResetAnimator()
        {
            _animator.Reset();
        }

        public void TeleportToRandomSpot()
        {
            float angle = GD.Randf() * Mathf.Deg2Rad(360);
            float radius = GD.Randf() * TeleportRadius;

            var parent = GetParent<Node2D>();
            parent.Position = new Vector2(
                radius * Mathf.Cos(angle),
                radius * Mathf.Sin(angle)
            );
        }
    }
}
