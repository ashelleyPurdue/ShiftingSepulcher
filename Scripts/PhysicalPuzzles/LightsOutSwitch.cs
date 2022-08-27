using System;
using Godot;

namespace RandomDungeons.PhysicalPuzzles
{
    public class LightsOutSwitch : Node2D
    {
        public event Action Activated;

        private void OnActivateTriggerEntered(object body)
        {
            if (body is Player)
                Activated?.Invoke();
        }

    }
}
