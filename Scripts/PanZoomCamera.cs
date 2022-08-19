using Godot;

namespace RandomDungeons
{
    public class PanZoomCamera : Node2D
    {
        public override void _Input(InputEvent e)
        {
            if (e is InputEventMouseMotion mouse && Input.IsActionPressed("ui_camera_pan"))
            {
                Position -= mouse.Relative;
            }
        }
    }
}
