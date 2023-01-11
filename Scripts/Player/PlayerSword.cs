using Godot;

namespace RandomDungeons
{
    public class PlayerSword : Node2D
    {
        public bool IsSwinging => _swingTimer > 0;

        private const float SwingDuration = 0.15f;
        private const float SwingAngleDeg = 135;
        private float SwingSpeed => SwingAngleDeg / SwingDuration;

        private HitBox _hitBox => GetNode<HitBox>("HitBox");
        private AudioStreamPlayer _swishSoundPlayer => GetNode<AudioStreamPlayer>("%SwordSwishSound");

        private float _swingTimer = 0;

        public override void _PhysicsProcess(float delta)
        {
            _swingTimer -= delta;
            RotationDegrees += SwingSpeed * delta;

            Visible = IsSwinging;
            _hitBox.Monitoring  = IsSwinging;
            _hitBox.Monitorable = IsSwinging;
        }

        public void StartSwinging(float angleDeg)
        {
            _swingTimer = SwingDuration;
            RotationDegrees = angleDeg - (SwingAngleDeg / 2);
            _swishSoundPlayer.Play();
        }
    }
}
