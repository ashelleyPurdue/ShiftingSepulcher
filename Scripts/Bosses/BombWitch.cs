using System;
using Godot;

namespace RandomDungeons
{
    public class BombWitch : Node
    {
        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%AnimationPlayer");

        public void TeleportAway()
        {
            _animator.ResetAndPlay("TeleportAway");
        }

        public void TeleportIn()
        {
            _animator.ResetAndPlay("TeleportIn");
        }

        public void Throw()
        {
            _animator.ResetAndPlay("Throw");
        }
    }
}
