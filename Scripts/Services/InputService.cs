using Godot;

namespace RandomDungeons.Services
{
    public static class InputService
    {
        public static Vector2 LeftStick => new Vector2(
            Input.GetAxis("MoveLeft", "MoveRight"),
            Input.GetAxis("MoveUp", "MoveDown")
        );

        public static bool ActivatePressed => Input.IsActionJustPressed("ui_accept");
    }
}