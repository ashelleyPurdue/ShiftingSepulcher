using Godot;

namespace ShiftingSepulcher
{
    public class BreakablePot : KinematicBody2D
    {
        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%AnimationPlayer");
        private bool _isDead = false;

        public void OnTookDamage(HitBox hitBox)
        {
            Shatter();
        }

        public void OnDealtDamage(HurtBox hurtBox)
        {
            Shatter();
        }

        public void Shatter()
        {
            if (_isDead)
                return;

            _isDead = true;
            _animator.PlaybackSpeed = 1;
            _animator.Play("Shatter");
        }
    }
}
