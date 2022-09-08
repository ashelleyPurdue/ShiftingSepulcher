using Godot;

using RandomDungeons.Services;
using RandomDungeons.Nodes.Components;
using RandomDungeons.Utils;

using RandomDungeons.StateMachines;
using RandomDungeons.StateMachines.CommonStates;

namespace RandomDungeons.Nodes.Elements
{
    public class Player : KinematicBody2D, IStateMachine
    {
        public const float WalkSpeed = 283;

        private EightDirectionalSprite _sprite => GetNode<EightDirectionalSprite>("%Sprite");
        private PlayerSword _sword => GetNode<PlayerSword>("%Sword");

        private IState _currentState;

        public override void _Ready()
        {
            PlayerInventory.Reset();

            DeathAnimation.AnimationTarget = _sprite;
            DeathAnimation.AnimationEnded += () => ChangeState(AfterDeathAnimation);

            ChangeState(Walking);
        }

        public void ChangeState(IState state)
        {
            if (state != null)
                state.Owner = this;

            var prevState = _currentState;
            _currentState = state;

            prevState?._StateExited();
            _currentState?._StateEntered();
        }

        public override void _Process(float deltaTime)
        {
            _currentState?._Process(deltaTime);
        }

        public override void _PhysicsProcess(float deltaTime)
        {
            _currentState?._PhysicsProcess(deltaTime);
        }

        public void OnTookDamage(HitBox hitBox)
        {
            PlayerInventory.Health -= hitBox.Damage;

            if (PlayerInventory.Health <= 0)
                ChangeState(DeathAnimation);
        }

        private readonly IState Walking = new WalkingState();
        private class WalkingState : State<Player>
        {
            public override void _Process(float delta)
            {
                if (InputService.AttackPressed && !Owner._sword.IsSwinging)
                    ChangeState(Owner.SwingingSword);
            }

            public override void _PhysicsProcess(float delta)
            {
                // Move with the left stick
                var cappedLeftStick = InputService.LeftStick.LimitLength(1);
                Owner.MoveAndSlide(cappedLeftStick * WalkSpeed);

                // Update the sprite
                if (cappedLeftStick.Length() > 0.01)
                    Owner._sprite.Direction = cappedLeftStick.ToNearestEightDirection();

                Owner._sprite.SpeedScale = cappedLeftStick.Length();
            }
        }

        private readonly IState SwingingSword = new SwingingSwordState();
        private class SwingingSwordState : State<Player>
        {
            public override void _StateEntered()
            {
                Owner._sword.StartSwinging(Owner._sprite.Direction);
                Owner._sprite.SpeedScale = 0;
            }

            public override void _PhysicsProcess(float delta)
            {
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
