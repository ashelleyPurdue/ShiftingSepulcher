using Godot;

namespace RandomDungeons
{
    public class TitleScreen : Control
    {
        public static int ChosenSeed;

        private LineEdit _seedTextBox => GetNode<LineEdit>("%SeedTextBox");

        public override void _Ready()
        {
            RandomizeSeed();
        }

        public void RandomizeSeed()
        {
            GD.Randomize();
            _seedTextBox.Text = "" + GD.Randi();
        }

        private void Play()
        {
            GetTree().ChangeScene("res://Maps/Main.tscn");
        }

    }
}


