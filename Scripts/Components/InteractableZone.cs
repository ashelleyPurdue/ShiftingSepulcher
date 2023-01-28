using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace RandomDungeons
{
    public class InteractableZone : Area2D
    {
        [Signal] public delegate void Interacted();

        [Export] public string PromptText = "";

        public override void _PhysicsProcess(float delta)
        {
            if (IsHighlighted() && InputService.ActivatePressed)
            {
                EmitSignal(nameof(Interacted));
            }
        }

        private bool IsHighlighted()
        {
            return GetTree().FindPlayer().HighlightedObject == this;
        }
    }
}
