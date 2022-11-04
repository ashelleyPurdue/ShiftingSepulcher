using Godot;

namespace RandomDungeons
{
    public static class InputService
    {
        public static Vector2 LeftStick => new Vector2(
            Input.GetAxis("MoveLeft", "MoveRight"),
            Input.GetAxis("MoveUp", "MoveDown")
        );

        public static bool PausePressed => Input.IsActionJustPressed("Pause");

        public static bool ActivatePressed => Input.IsActionJustPressed("ui_accept");
        public static bool AttackPressed => Input.IsActionJustPressed("Attack");
    }
}
