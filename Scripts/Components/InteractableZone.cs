using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace RandomDungeons
{
    public class InteractableZone : Area2D, IInteractable
    {
        [Signal] public delegate void Interacted();

        [Export] public string PromptText {get; set;} = "";
        [Export] public NodePath PromptPositionPath;

        public Vector2 PromptGlobalPosition => PromptPositionPath != null
            ? GetNode<Node2D>(PromptPositionPath).GlobalPosition
            : GlobalPosition;

        public void Interact()
        {
            EmitSignal(nameof(Interacted));
        }
    }
}
