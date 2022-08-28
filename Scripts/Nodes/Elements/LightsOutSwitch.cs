using System;
using Godot;

using RandomDungeons.Services;

namespace RandomDungeons.Nodes.Elements
{
    public class LightsOutSwitch : Node2D
    {
        public event Action Activated;

        private bool _isBeingHighlighted = false;

        public override void _Process(float delta)
        {
            if (_isBeingHighlighted && InputService.ActivatePressed)
                Activated?.Invoke();
        }

        private void OnActivateTriggerEntered(object body)
        {
            if (body is Player)
                _isBeingHighlighted = true;
        }

        private void OnActivateTriggerExited(object body)
        {
            if (body is Player)
                _isBeingHighlighted = false;
        }
    }
}
