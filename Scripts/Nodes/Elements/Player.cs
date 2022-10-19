using Godot;

using RandomDungeons.Services;
using RandomDungeons.Nodes.Components;
using RandomDungeons.Utils;

using RandomDungeons.StateMachines;
using RandomDungeons.StateMachines.CommonStates;

namespace RandomDungeons.Nodes.Elements
{
    public class Player : KinematicBody2D
    {
        public const float WalkSpeed = 283;
        public const float WalkAccel = WalkSpeed / 0.0625f;
        public const float KnockbackFriction = WalkSpeed / 0.125f;

        [Signal] public delegate void DeathAnimationFinished();

        /// <summary>
        /// Set this to false during cutscenes, dialog, etc. to prevent the
        /// player from doing stuff.
        ///
        /// Please use this sparingly.
        /// </summary>
        public bool ControlsEnabled = true;

        private Node2D _visuals => GetNode<Node2D>("%Visuals");
        private PlayerSword _sword => GetNode<PlayerSword>("%Sword");
        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%AnimationPlayer");
        private HurtFlasher _hurtFlasher => GetNode<HurtFlasher>("%HurtFlasher");

        private Vector2 _velocity;

        private bool _isDead = false;
        private float _knockbackTimer = 0;

        private float _accel => _knockbackTimer > 0
            ? KnockbackFriction
            : WalkAccel;

        private StateMachine _sm;

        public override void _Ready()
        {
            _sm = new StateMachine(this);
            _sm.ChangeState(Spawning);
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);
            _knockbackTimer -= delta;

            // Move
            _velocity = MoveAndSlide(_velocity);

            // Die when out of health
            if (PlayerInventory.Health <= 0 && !_isDead)
            {
                _sm.ChangeState(Dead);
            }
        }

        public void EmitDeathAnimationFinished()
        {
            EmitSignal(nameof(DeathAnimationFinished));
        }

        /// <summary>
        /// Resurrects the player on-the-spot, if they're dead.
        /// This does NOT move the player back to their spawn point; the caller
        /// is responsible for that.
        /// </summary>
        public void Resurrect()
        {
            _isDead = false;
            PlayerInventory.Health = 3;

            _visuals.Scale = Vector2.One;
            _visuals.Modulate = Colors.White;

            _velocity = Vector2.Zero;
            _sm.ChangeState(Spawning);
        }

        public void OnTookDamage(HitBox hitBox)
        {
            if (_isDead)
                return;

            PlayerInventory.Health -= hitBox.Damage;
            _hurtFlasher.Flash();

            _velocity = hitBox.GetKnockbackVelocity(this, KnockbackFriction);
            _knockbackTimer = KnockbackDuration(_velocity);
        }

        public void OnSwordDealtDamage(HurtBox hurtBox)
        {
            _velocity = hurtBox.GetRecoilVelocity(this, KnockbackFriction);
            _knockbackTimer = KnockbackDuration(_velocity);
        }

        private float KnockbackDuration(Vector2 velocity)
        {
            return velocity.Length() / KnockbackFriction;
        }

        private readonly IState Walking = new WalkingState();
        private class WalkingState : State<Player>
        {
            public override void _StateEntered()
            {
                Owner._animator.CurrentAnimation = "WalkAnimation";
            }

            public override void _Process(float delta)
            {
                if (!Owner.ControlsEnabled)
                {
                    Owner._animator.PlaybackSpeed = 0;
                    return;
                }

                Owner._animator.PlaybackSpeed = InputService.LeftStick
                    .LimitLength(1)
                    .Length();

                if (InputService.AttackPressed && !Owner._sword.IsSwinging)
                    ChangeState(Owner.SwingingSword);
            }

            public override void _PhysicsProcess(float delta)
            {
                if (!Owner.ControlsEnabled)
                {
                    Owner._velocity = Vector2.Zero;
                    return;
                }

                // Move with the left stick
                var cappedLeftStick = InputService.LeftStick.LimitLength(1);
                var desiredVelocity = cappedLeftStick * WalkSpeed;
                Owner._velocity = Owner._velocity.MoveToward(
                    desiredVelocity,
                    Owner._accel * delta
                );

                // Update the rotation of the visuals
                if (cappedLeftStick.Length() > 0.01)
                    Owner._visuals.Rotation = cappedLeftStick.Angle();
            }

            public override void _StateExited()
            {
                Owner._animator.PlaybackSpeed = 0;
            }
        }

        private readonly IState SwingingSword = new SwingingSwordState();
        private class SwingingSwordState : State<Player>
        {
            public override void _StateEntered()
            {
                Owner._sword.StartSwinging(Owner._visuals.RotationDegrees);
                Owner._velocity = Vector2.Zero;
            }

            public override void _PhysicsProcess(float delta)
            {
                Owner._velocity = Owner._velocity.MoveToward(
                    Vector2.Zero,
                    WalkAccel * delta
                );

                if (!Owner._sword.IsSwinging)
                    ChangeState(Owner.Walking);
            }
        }

        private readonly IState Dead = new DeadState();
        private class DeadState : State<Player>
        {
            public override void _StateEntered()
            {
                Owner._isDead = true;
                Owner._hurtFlasher.Cancel();
                Owner._velocity = Vector2.Zero;

                Owner._animator.PlaybackSpeed = 1;
                Owner._animator.Play("Die");
            }
        }

        private readonly IState Spawning = new SpawningState();
        private class SpawningState : State<Player>
        {
            public override void _StateEntered()
            {
                Owner._animator.PlaybackSpeed = 1;
                Owner._animator.Play("Spawn");
            }

            public override void _Process(float delta)
            {
                if (!Owner._animator.IsPlaying())
                    ChangeState(Owner.Walking);
            }
        }
    }
}
