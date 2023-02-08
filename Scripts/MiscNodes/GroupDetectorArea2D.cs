using Godot;

namespace RandomDungeons
{
    [CustomNode(parent: "Area2D", icon: "Area2D")]
    public class GroupDetectorArea2D : Area2D
    {
        [Signal] public delegate void GroupAreaEntered(Area2D area);
        [Signal] public delegate void GroupAreaEnteredNoParams();

        [Signal] public delegate void GroupAreaExited(Area2D area);
        [Signal] public delegate void GroupAreaExitedNoParams();

        // TODO: Support bodies, too, not just other areas.

        [Export] public string[] GroupsToDetect = new string[] {};
        [Export] public string[] GroupsToIgnore = new string[] {};
        [Export] public bool CheckAncestors = true;

        public override void _Ready()
        {
            Connect("area_entered", this, nameof(OnAreaEntered));
            Connect("area_exited", this, nameof(OnAreaExited));
        }

        private void OnAreaEntered(Area2D area)
        {
            if (ShouldDetect(area))
            {
                EmitSignal(nameof(GroupAreaEntered), area);
                EmitSignal(nameof(GroupAreaEnteredNoParams));
            }
        }

        private void OnAreaExited(Area2D area)
        {
            if (ShouldDetect(area))
            {
                EmitSignal(nameof(GroupAreaExited), area);
                EmitSignal(nameof(GroupAreaExitedNoParams));
            }
        }

        private bool ShouldDetect(Node node)
        {
            // Reject if any of the ignored groups are present
            foreach (var group in GroupsToIgnore)
            {
                if (InGroup(node, group))
                    return false;
            }

            // Accept if any of the detected groups are present
            foreach (var group in GroupsToDetect)
            {
                if (InGroup(node, group))
                    return true;
            }

            // Reject if none of the detected groups are present
            return false;
        }

        private bool InGroup(Node node, string group)
        {
            return CheckAncestors
                ? node.IsAncestorInGroup(group)
                : node.IsInGroup(group);
        }
    }
}
