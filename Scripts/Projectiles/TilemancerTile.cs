using Godot;

namespace ShiftingSepulcher
{
    public class TilemancerTile : KinematicBody2D
    {
        private Node2D _target;
        private CollisionShape2D _collider => GetNode<CollisionShape2D>("%Collider");
        private HitBox _hitBox => GetNode<HitBox>("%HitBox");
        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%AnimationPlayer");
        private AudioStreamPlayer _throwSoundPlayer => GetNode<AudioStreamPlayer>("%ThrowSound");

        private Vector2 _velocity;

        public override void _Ready()
        {
            _target = GetTree().FindPlayer();
        }

        public override void _PhysicsProcess(float delta)
        {
            var collision = MoveAndCollide(_velocity * delta);

            if (collision != null)
                Shatter();
        }

        public void Throw(float speed)
        {
            Vector2 dir = GlobalPosition.DirectionTo(_target.GlobalPosition);
            _velocity = dir * speed;

            _collider.Disabled = false;
            _hitBox.Monitoring = true;
            _throwSoundPlayer.Play();
        }

        public void Shatter()
        {
            _animator.Play("Shatter");
            _hitBox.Monitoring = false;
            _collider.Disabled = true;
            _velocity = Vector2.Zero;
        }

        public void OnDealtDamage(HurtBox hurtBox) => Shatter();
        public void OnTookDamage(HitBox hitBox) => Shatter();
    }
}
