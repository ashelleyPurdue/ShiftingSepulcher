using System;
using Godot;

namespace RandomDungeons
{
    public class BombWitch : Node
    {
        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%AnimationPlayer");

        public void ResetAnimator()
        {
            _animator.Reset();
        }
    }
}
