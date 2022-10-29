using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace RandomDungeons
{
    public class InteractableZone : Area2D
    {
        [Signal] public delegate void Interacted();

        public override void _Process(float delta)
        {
            if (IsHighlighted() && InputService.ActivatePressed)
            {
                EmitSignal(nameof(Interacted));
            }
        }

        private bool IsHighlighted()
        {
            return GetOverlappingBodies()
                .OfType<Node>()
                .Any(n => n is Player);
        }
    }
}