using System;
using Godot;

namespace RandomDungeons.StateMachines.CommonStates
{
    public class KnockedBackState<TOwner> : State<TOwner>
        where TOwner : KinematicBody2D, IStateMachine
    {
        private const float Friction = 500;
        private const float MinSpeedForCollisionDamage = 90;

        public event Action HitWall;
        public event Action StoppedMoving;

        public Vector2 Velocity;

        public override void _PhysicsProcess(float delta)
        {
            // Move
            var prevVel = Velocity;
            Velocity = Owner.MoveAndSlide(Velocity);
            Velocity = Velocity.MoveToward(Vector2.Zero, Friction * delta);

            // Take damage upon hitting a wall too hard
            bool fastEnough = prevVel.Length() > MinSpeedForCollisionDamage;
            if (Owner.GetSlideCount() > 0 && fastEnough)
                HitWall?.Invoke();

            // Resume normal behavior after coming to a stop
            if (Velocity == Vector2.Zero)
                StoppedMoving?.Invoke();
        }
    }
}
