using System;
using Godot;

namespace RandomDungeons
{
    public class CarryableParent : Area2D, ICarryable
    {
        [Signal] public delegate void PickedUp();
        [Signal] public delegate void Released();

        public Node2D Node => GetParent<Node2D>();

        private Node _originalParent;
        private bool _isBeingCarried = false;

        public void PickUp(Node2D carrier)
        {
            if (_isBeingCarried)
                throw new InvalidOperationException($"{Node.Name} is already being carried");

            _isBeingCarried = true;

            _originalParent = Node.GetParent();
            _originalParent.RemoveChild(Node);
            carrier.AddChild(Node);

            Node.Position = Vector2.Zero;
            EmitSignal(nameof(PickedUp));
        }

        public void Release(Vector2 releasePosGlobal)
        {
            if (!_isBeingCarried)
                throw new InvalidOperationException($"{Node.Name} is not being carried");

            _isBeingCarried = false;

            Node.GetParent().RemoveChild(Node);
            _originalParent.AddChild(Node);

            Node.GlobalPosition = releasePosGlobal;
            EmitSignal(nameof(Released));
        }
    }
}
