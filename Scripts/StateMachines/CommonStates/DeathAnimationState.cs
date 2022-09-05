using System;
using Godot;

namespace RandomDungeons.StateMachines.CommonStates
{
    public class DeathAnimationState : State<IStateMachine>
    {
        private const float EndScaleMultiplier = 2;
        private const float AnimationDuration = 0.1f;

        public Node2D AnimationTarget;
        public event Action AnimationEnded;

        private bool _firedAnimationEnded;
        private float _startAlpha;
        private Vector2 _startScale;
        private Vector2 _endScale;

        public override void _StateEntered()
        {
            _firedAnimationEnded = false;

            _startAlpha = AnimationTarget.Modulate.a;
            _startScale = AnimationTarget.Scale;
            _endScale = _startScale * EndScaleMultiplier;
        }

        public override void _Process(float delta)
        {
            float fadeSpeed = _startAlpha / AnimationDuration;
            float scaleSpeed = (_startScale.DistanceTo(_endScale)) / AnimationDuration;

            // Tween the alpha
            Color c = AnimationTarget.Modulate;
            c.a = Mathf.MoveToward(c.a, 0, fadeSpeed * delta);
            AnimationTarget.Modulate = c;

            // Tween the scale
            AnimationTarget.Scale = AnimationTarget.Scale.MoveToward(
                _endScale,
                scaleSpeed * delta
            );

            if (AnimationTarget.Scale == _endScale && !_firedAnimationEnded)
            {
                _firedAnimationEnded = true;
                AnimationEnded?.Invoke();
            }
        }
    }
}
