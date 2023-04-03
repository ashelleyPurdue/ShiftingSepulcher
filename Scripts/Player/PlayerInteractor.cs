using Godot;

namespace ShiftingSepulcher
{
    public class PlayerInteractor : Node2D
    {
        public InteractableComponent HighlightedObject {get; private set;} = null;
        public bool IsObjectHighlighted => IsInstanceValid(HighlightedObject as Node);

        private Area2D _interactableDetector => GetNode<Area2D>("%InteractableDetector");

        public override void _PhysicsProcess(float delta)
        {
            UpdateHighlightedObject();
        }

        public void TryInteract()
        {
            if (!IsInstanceValid(HighlightedObject as Node))
                return;

            HighlightedObject.Interact();
        }

        private void UpdateHighlightedObject()
        {
            var hits = _interactableDetector.GetOverlappingAreas();

            foreach (var hit in hits)
            {
                if (((Node)hit).HasComponent<InteractableComponent>(out var i))
                {
                    if (!i.InteractEnabled)
                        continue;

                    HighlightedObject = i;
                    return;
                }
            }

            HighlightedObject = null;
        }
    }
}
