using Godot;
using RandomDungeons.StateMachines;
using RandomDungeons.StateMachines.CommonStates;
using RandomDungeons.Nodes.Components;

namespace RandomDungeons.Nodes.Elements.Enemies
{
    public abstract class BaseEnemy : KinematicBody2D, IStateMachine
    {
        [Export] public int Health;

        protected abstract Node2D Visuals();
        protected abstract HurtBox Hurtbox();
        protected abstract IState InitialState();

        private IState _currentState;

        private readonly KnockedBackState<BaseEnemy> KnockedBack = new KnockedBackState<BaseEnemy>();
        private readonly DeathAnimationState DeathAnimation = new DeathAnimationState();

        public override void _Ready()
        {
            KnockedBack.HitWall += () =>
            {
                Health--;
                OnHitWall();
            };
            KnockedBack.StoppedMoving += OnKnockbackFinished;

            DeathAnimation.AnimationEnded += QueueFree;
            DeathAnimation.AnimationTarget = Visuals();

            Hurtbox().Connect(
                signal: nameof(HurtBox.TookDamage),
                target: this,
                method: nameof(OnTookDamage)
            );

            ChangeState(InitialState());
        }

        public void ChangeState(IState state)
        {
            if (state != null)
            {
                state.StateMachine = this;
                state.Owner = this;
            }

            var prevState = _currentState;
            _currentState = state;

            prevState?._StateExited();
            _currentState?._StateEntered();
        }

        public override void _Process(float delta)
        {
            _currentState?._Process(delta);
        }

        public override void _PhysicsProcess(float delta)
        {
            _currentState?._PhysicsProcess(delta);

            if (Health <= 0 && _currentState != DeathAnimation)
            {
                ChangeState(DeathAnimation);
            }
        }

        protected virtual void OnTookDamage(HitBox hitBox)
        {
            Health -= hitBox.Damage;
            KnockedBack.Velocity = hitBox.GetKnockbackVelocity(this);

            ChangeState(KnockedBack);
        }

        protected virtual void OnHitWall() => ChangeState(InitialState());
        protected virtual void OnKnockbackFinished() => ChangeState(InitialState());
    }
}
