using Godot;

namespace RandomDungeons
{
    public class PlayerInteractor : Node2D
    {
        public InteractableZone HighlightedObject {get; private set;} = null;

        // TODO: Rename the node
        private Area2D _interactableDetector => GetNode<Area2D>("%HoldableDetector");

        public override void _PhysicsProcess(float delta)
        {
            UpdateHighlightedObject();
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
