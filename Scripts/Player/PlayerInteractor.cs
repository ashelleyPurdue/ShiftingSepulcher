using Godot;

namespace RandomDungeons
{
    public class PlayerInteractor : Node2D
    {
        public InteractableZone HighlightedObject {get; private set;} = null;

        private Area2D _interactableDetector => GetNode<Area2D>("%InteractableDetector");

        public override void _PhysicsProcess(float delta)
        {
            UpdateHighlightedObject();
        }

        public void TryInteract()
        {
            if (!Object.IsInstanceValid(HighlightedObject))
                return;

            HighlightedObject.Interact();
        }

        private void UpdateHighlightedObject()
        {
            var hits = _interactableDetector.GetOverlappingAreas();

            foreach (var hit in hits)
            {
                if (hit is InteractableZone i)
                {
                    HighlightedObject = i;
                    return;
                }
            }

            HighlightedObject = null;
        }
    }
}
