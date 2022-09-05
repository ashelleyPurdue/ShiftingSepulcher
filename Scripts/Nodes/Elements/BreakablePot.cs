using Godot;

using RandomDungeons.Nodes.Components;
using RandomDungeons.StateMachines;
using RandomDungeons.StateMachines.CommonStates;

namespace RandomDungeons.Nodes.Elements
{
    public class BreakablePot : KinematicBody2D, IStateMachine
    {
        [Export] public int Health = 2;

        private float _invulnerableTimer = 0;
        private IState _currentState = null;

        private readonly KnockedBackState<BreakablePot> KnockedBack = new KnockedBackState<BreakablePot>();
        private readonly DeathAnimationState DeathAnimation = new DeathAnimationState();

        public override void _Ready()
        {
            KnockedBack.StoppedMoving += () => ChangeState(null);
            KnockedBack.HitWall += () =>
            {
                Health--;
                ChangeState(null);
            };

            DeathAnimation.AnimationEnded += () => QueueFree();
            DeathAnimation.AnimationTarget = GetNode<Node2D>("%Visuals");
        }

        public void ChangeState(IState state)
        {
            state.Owner = this;

            var prevState = _currentState;
            _currentState = state;

            prevState?._StateExited();
            _currentState._StateEntered();
        }

        public override void _Process(float delta)
        {
            _currentState?._Process(delta);
        }

        public override void _PhysicsProcess(float delta)
        {
            _currentState?._PhysicsProcess(delta);
            _invulnerableTimer -= delta;

            if (Health <= 0 && _currentState != DeathAnimation)
            {
                ChangeState(DeathAnimation);
            }
        }

        public void OnTookDamage(HitBox hitBox)
        {
            if (_invulnerableTimer > 0)
                return;

            Health -= hitBox.Damage;
            _invulnerableTimer = hitBox.InvlunerabilityTime;

            KnockedBack.Velocity = hitBox.KnockbackVelocity;
            ChangeState(KnockedBack);
        }
    }
}
