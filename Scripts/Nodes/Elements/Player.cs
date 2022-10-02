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
        public const float WalkAccel = WalkSpeed / 0.125f;

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

        private StateMachine _sm;

        public override void _Ready()
        {
            PlayerInventory.Reset();

            DeathAnimation.AnimationTarget = _visuals;
            DeathAnimation.AnimationEnded += () => _sm.ChangeState(AfterDeathAnimation);

            _sm = new StateMachine(this);
            _sm.ChangeState(Walking);
        }

        public override void _PhysicsProcess(float delta)
        {
            base._PhysicsProcess(delta);

            // Move
            _velocity = MoveAndSlide(_velocity);

            // Die when out of health
            if (PlayerInventory.Health <= 0 && !_isDead)
            {
                _isDead = true;
                _hurtFlasher.Cancel();
                _velocity = Vector2.Zero;

                _sm.ChangeState(DeathAnimation);
            }
        }

        public void OnTookDamage(HitBox hitBox)
        {
            if (_isDead)
                return;

            PlayerInventory.Health -= hitBox.Damage;
            _hurtFlasher.Flash();

            _velocity = hitBox.GetKnockbackVelocity(this, WalkAccel);
        }

        public void OnSwordDealtDamage(HurtBox hurtBox)
        {
            _velocity = hurtBox.GetRecoilVelocity(this, WalkAccel);
        }

        private readonly IState Walking = new WalkingState();
        private class WalkingState : State<Player>
        {
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
                    WalkAccel * delta
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

        private readonly IState AfterDeathAnimation = new AfterDeathAnimationState();
        private class AfterDeathAnimationState : State<Player>
        {
            private const float Duration = 2;
            private float _timer;

            public override void _StateEntered()
            {
                _timer = Duration;
            }

            public override void _Process(float delta)
            {
                // Hang out for a little bit before going back to the title
                // screen
                _timer -= delta;

                if (_timer < 0)
                {
                    // TODO: Respawn the player in the starting room, instead
                    // of taking them back to the title screen
                    Owner.GetTree().ChangeScene("res://Maps/TitleScreen.tscn");
                }
            }
        }

        private readonly DeathAnimationState DeathAnimation = new DeathAnimationState();
    }
}
