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

        private HoldableComponent _heldObject;
        private float _originalGlobalRotation;

        public override void _PhysicsProcess(float delta)
        {
            if (!IsHoldingSomething)
                return;

            _heldObject.Entity.GlobalPosition = _holdPos.GlobalPosition;

            if (_heldObject.RotatesWhileHeld)
                _heldObject.Entity.GlobalRotation = _holdPos.GlobalRotation;
        }

        public void PickUp(HoldableComponent holdable)
        {
            if (IsHoldingSomething)
                throw new Exception("Already holding something");

            _heldObject = holdable;
            _originalGlobalRotation = holdable.Entity.GlobalRotation;

            holdable.PickUp();
        }

        public void ReleaseHeldObject()
        {
            if (!IsHoldingSomething)
                return;

            _heldObject.Entity.GlobalPosition = _releasePos.GlobalPosition;
            _heldObject.Entity.GlobalRotation = _originalGlobalRotation;

            _heldObject.Release();
            _heldObject = null;
        }

        public void ThrowHeldObject(Vector2 direction)
        {
            if (!IsHoldingSomething)
                return;

            _heldObject.Entity.GlobalPosition = _releasePos.GlobalPosition;
            _heldObject.Entity.GlobalRotation = _originalGlobalRotation;

            _heldObject.Throw(direction);
            _heldObject = null;
        }
    }
}
