using Godot;

namespace ShiftingSepulcher
{
    public class Chompweed : Node2D
    {
        [Export] public float TrackingTargetDuration = 1;
        [Export] public float LungeDuration = 0.25f;
        [Export] public float PauseAfterLungeDuration = 0.5f;
        [Export] public float RecoverDuration = 0.5f;

        private readonly IState Idle = new IdleState();
        private class IdleState : State<Chompweed> {}

        private readonly IState TrackingTarget = new TrackingTargetState();
        private class TrackingTargetState : State<Chompweed> {}

        private readonly IState Lunging = new LungingState();
        private class LungingState : State<Chompweed> {}

        private readonly IState PausingAfterLunge = new PausingAfterLungeState();
        private class PausingAfterLungeState : State<Chompweed> {}

        private readonly IState Recovering = new RecoveringState();
        private class RecoveringState : State<Chompweed> {}
    }
}
