using Godot;
using RandomDungeons.StateMachines;
using RandomDungeons.StateMachines.CommonStates;
using RandomDungeons.Nodes.Components;

namespace RandomDungeons.Nodes.Elements.Enemies
{
    public abstract class BaseEnemy : KinematicBody2D
    {
        [Export] public int Health;

        protected abstract Node2D Visuals();
        protected abstract HurtBox Hurtbox();
        protected abstract IState InitialState();

        protected StateMachine _sm;

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

            _sm = new StateMachine(this);
            _sm.ChangeState(InitialState());
        }

        public override void _PhysicsProcess(float delta)
        {
            if (Health <= 0 && _sm.CurrentState != DeathAnimation)
                _sm.ChangeState(DeathAnimation);
        }

        protected virtual void OnTookDamage(HitBox hitBox)
        {
            Health -= hitBox.Damage;
            KnockedBack.Velocity = hitBox.GetKnockbackVelocity(this);

            _sm.ChangeState(KnockedBack);
        }

        protected virtual void OnHitWall() => _sm.ChangeState(InitialState());
        protected virtual void OnKnockbackFinished() => _sm.ChangeState(InitialState());
    }
}
