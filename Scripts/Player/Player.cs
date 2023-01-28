using System.Linq;
using Godot;

namespace RandomDungeons
{
    public class Player : KinematicBody2D
    {
        public const float WalkSpeed = 283;
        public const float WalkAccel = WalkSpeed / 0.0625f;
        public const float KnockbackFriction = WalkSpeed / 0.125f;

        [Signal] public delegate void DeathAnimationFinished();

        public InteractableZone HighlightedObject => _interactor.HighlightedObject;

        public float FacingAngleRadians
        {
            get => _visuals.Rotation;
            set => _visuals.Rotation = value;
        }

        public Vector2 FacingDirection => new Vector2(
            Mathf.Cos(FacingAngleRadians),
            Mathf.Sin(FacingAngleRadians)
        );

        private Node2D _visuals => GetNode<Node2D>("%Visuals");
        private PlayerSword _sword => GetNode<PlayerSword>("%Sword");
        private ObjectHolder _objectHolder => GetNode<ObjectHolder>("%ObjectHolder");
        private PlayerInteractor _interactor => GetNode<PlayerInteractor>("%Interactor");
        private AnimationPlayer _animator => GetNode<AnimationPlayer>("%AnimationPlayer");
        private HurtFlasher _hurtFlasher => GetNode<HurtFlasher>("%HurtFlasher");

        private Vector2 _velocity;

        private bool _frozenForCutscene = false;
        private bool _isDead = false;
        private float _knockbackTimer = 0;

        private float _accel => _knockbackTimer > 0
            ? KnockbackFriction
            : WalkAccel;

        private StateMachine _sm;

        public override void _Ready()
        {
            _sm = new StateMachine(this);
            Resurrect();
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
        /// Call this during cutscenes, dialog, etc. to prevent the
        /// player from doing stuff.  Call <see cref="UnfreezeForCutscene"/>
        /// to enable controls again.
        ///
        /// Please use this sparingly, because players HATE being stuck in
        /// cutscenes.
        ///
        /// Only one "thing" is allowed to freeze the player at a time.
        /// If anything else tries to freeze the player while they're already
        /// frozen, an exception will be thrown.
        ///
        /// This is to prevent the following scenario:
        /// * Cutscene A starts and freezes the player
        /// * Cutscene B starts and freezes the player
        /// * Cutscene A finishes and unfreezes the player
        /// * The player can now move even though Cutscene B is still playing
        ///
        /// </summary>
        public void FreezeForCutscene()
        {
            if (_frozenForCutscene)
                throw new System.Exception("The player is already frozen");

            _frozenForCutscene = true;
        }

        public void UnfreezeForCutscene()
        {
            _frozenForCutscene = false;
        }

        /// <summary>
        /// Resurrects the player on-the-spot, if they're dead.
        /// This does NOT move the player back to their spawn point; the caller
        /// is responsible for that.
        /// </summary>
        public void Resurrect()
        {
            _isDead = false;
            PlayerInventory.FullHeal();

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

            if (PlayerInventory.Health >= 0)
                GetNode<AudioStreamPlayer>("%HurtSound").Play();
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

        private void TryPickUpHoldable()
        {
            var holdable = GetNode<Area2D>("%HoldableDetector")
                .GetOverlappingAreas()
                .Cast<Area2D>()
                .OfType<IHoldable>()
                .FirstOrDefault();

            if (holdable == null)
                return;

            _objectHolder.PickUp(holdable);
        }

        public void ReleaseHeldObject()
        {
            _objectHolder.ReleaseHeldObject();
        }

        public void ThrowHeldObject()
        {
            _objectHolder.ThrowHeldObject(FacingDirection);
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
                if (Owner._frozenForCutscene)
                {
                    Owner._animator.PlaybackSpeed = 0;
                    return;
                }

                Owner._animator.PlaybackSpeed = InputService.LeftStick
                    .LimitLength(1)
                    .Length();

                if (Owner._objectHolder.IsHoldingSomething)
                {
                    if (InputService.ActivatePressed)
                        Owner.ReleaseHeldObject();
                    else if (InputService.AttackPressed)
                        Owner.ThrowHeldObject();

                    return;
                }

                if (InputService.ActivatePressed)
                    Owner.TryPickUpHoldable();

                if (InputService.AttackPressed)
                    ChangeState(Owner.SwingingSword);
            }

            public override void _PhysicsProcess(float delta)
            {
                if (Owner._frozenForCutscene)
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
                    Owner.FacingAngleRadians = cappedLeftStick.Angle();
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
                Owner.GetNode<AudioStreamPlayer>("%DeathSound").Play();

                MusicService.Instance.StopSong();
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
