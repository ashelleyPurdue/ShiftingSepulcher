using System;
using Godot;

namespace RandomDungeons
{
    public class HoldableParent : InteractableZone, IHoldable
    {
        [Signal] public delegate void PickedUp();
        [Signal] public delegate void Released();
        [Signal] public delegate void Thrown(Vector2 direction);

        public Node2D Node => GetParent<Node2D>();

        [Export] public bool RotatesWhileHeld {get; set;} = false;

        public bool IsBeingHeld {get; private set;} = false;

        public override void _Ready()
        {
            Connect(
                signal: nameof(Interacted),
                target: this,
                method: nameof(OnInteracted)
            );

            PromptText = "Pick up";
        }

        public void OnInteracted()
        {
            GetTree().FindPlayer().PickUp(this);
        }

        public void PickUp()
        {
            if (IsBeingHeld)
                throw new InvalidOperationException($"{Node.Name} is already being held");

            IsBeingHeld = true;
            EmitSignal(nameof(PickedUp));
        }

        public void Release()
        {
            if (!IsBeingHeld)
                throw new InvalidOperationException($"{Node.Name} is not being held");

            IsBeingHeld = false;
            EmitSignal(nameof(Released));
        }

        public void Throw(Vector2 direction)
        {
            Release();
            EmitSignal(nameof(Thrown), direction);
        }
    }
}
