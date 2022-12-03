using System;
using System.Linq;
using Godot;

namespace RandomDungeons
{
    public class ObjectHolder : Node2D
    {
        [Export] public NodePath HoldPos;
        [Export] public NodePath ReleasePos;

        public bool IsHoldingSomething => _heldObject is Godot.Object gdObj
            ? IsInstanceValid(gdObj)
            : _heldObject != null;

        private Node2D _holdPos => GetNode<Node2D>(HoldPos);
        private Node2D _releasePos => GetNode<Node2D>(ReleasePos);

        private IHoldable _heldObject;

        public void PickUp(IHoldable holdable)
        {
            if (IsHoldingSomething)
                throw new Exception("Already holding something");

            _heldObject = holdable;
            holdable.PickUp(_holdPos);
        }

        public void ReleaseHeldObject()
        {
            if (!IsHoldingSomething)
                return;

            _heldObject.Release(_releasePos.GlobalPosition);
            _heldObject = null;
        }

        public void ThrowHeldObject(Vector2 direction)
        {
            if (!IsHoldingSomething)
                return;

            _heldObject.Throw(_releasePos.GlobalPosition, direction);
            _heldObject = null;
        }
    }
}
