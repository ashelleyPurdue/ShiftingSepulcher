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

        public void Interact()
        {
            EmitSignal(nameof(Interacted));
        }
    }
}
