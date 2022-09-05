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
            DeathAnimation.AnimationTarget = _sprite;
            DeathAnimation.AnimationEnded += () =>
            {
                GetTree().ChangeScene("res://Maps/TitleScreen.tscn");
            };

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
            _currentState._PhysicsProcess(deltaTime);
        }

        public void OnTookDamage(HitBox hitBox)
        {
            // TODO: Take more than one hit to die
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

        private readonly DeathAnimationState DeathAnimation = new DeathAnimationState();

    }
}
