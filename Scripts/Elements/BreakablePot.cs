using Godot;

namespace RandomDungeons
{
    public class BreakablePot : Node2D
    {
        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%AnimationPlayer");
        private bool _isDead = false;

        public void OnTookDamage(HitBox hitBox)
        {
            if (!_isDead)
            {
                _isDead = true;
                _animator.Play("Shatter");
            }
        }
    }
}
