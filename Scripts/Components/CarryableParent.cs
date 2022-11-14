using System;
using Godot;

namespace RandomDungeons
{
    public class CarryableParent : Area2D, ICarryable
    {
        [Signal] public delegate void PickedUp();
        [Signal] public delegate void Released();
        [Signal] public delegate void Thrown(Vector2 direction);

        public Node2D Node => GetParent<Node2D>();

        [Export] public bool RotatesWhileCarried = true;

        public bool IsBeingCarried {get; private set;} = false;

        private Node _originalParent;
        private float _originalGlobalRotation;

        public override void _Process(float delta)
        {
            if (IsBeingCarried && !RotatesWhileCarried)
                Node.GlobalRotation = _originalGlobalRotation;
        }

        public void PickUp(Node2D carrier)
        {
            if (IsBeingCarried)
                throw new InvalidOperationException($"{Node.Name} is already being carried");

            IsBeingCarried = true;

            _originalGlobalRotation = Node.GlobalRotation;
            _originalParent = Node.GetParent();

            _originalParent.RemoveChild(Node);
            carrier.AddChild(Node);

            Node.Position = Vector2.Zero;

            EmitSignal(nameof(PickedUp));
        }

        public void Release(Vector2 releasePosGlobal)
        {
            if (!IsBeingCarried)
                throw new InvalidOperationException($"{Node.Name} is not being carried");

            IsBeingCarried = false;

            Node.GetParent().RemoveChild(Node);
            _originalParent.AddChild(Node);

            Node.GlobalRotation = _originalGlobalRotation;
            Node.GlobalPosition = releasePosGlobal;

            EmitSignal(nameof(Released));
        }

        public void Throw(Vector2 releasePosGlobal, Vector2 direction)
        {
            Release(releasePosGlobal);
            EmitSignal(nameof(Thrown), direction);
        }
    }
}
