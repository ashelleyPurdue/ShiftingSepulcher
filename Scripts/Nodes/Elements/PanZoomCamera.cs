using Godot;

namespace RandomDungeons.Nodes.Elements
{
    public class PanZoomCamera : Camera2D
    {
        public override void _Input(InputEvent e)
        {
            if (e is InputEventMouseMotion mouse && Input.IsActionPressed("ui_camera_pan"))
            {
                Position -= mouse.Relative * this.Zoom;
            }
            else if (e is InputEventMouseButton mouseButton)
            {
                if (!mouseButton.IsPressed())
                    return;

                if (mouseButton.ButtonIndex == (int)ButtonList.WheelUp)
                    this.Zoom *= 9f / 10;
                else if (mouseButton.ButtonIndex == (int)ButtonList.WheelDown)
                    this.Zoom *= 10f / 9;
            }
        }
    }
}
