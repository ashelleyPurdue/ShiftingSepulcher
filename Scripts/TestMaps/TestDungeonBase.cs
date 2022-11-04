using Godot;

namespace RandomDungeons
{
    public class TestDungeonBase : Node
    {
        private DungeonInstantiator _instantiator => GetNode<DungeonInstantiator>("%DungeonInstantiator");
        private SeedInput _seedInput => GetNode<SeedInput>("%SeedInput");
        private Node2D _player => GetNode<Node2D>("%Player");

        public override void _Ready()
        {
            _player.Visible = false;
            _player.SetPaused(true);
        }

        public void Generate()
        {
            TitleScreen.ChosenSeed = _seedInput.ParseSeedTextbox();
            _instantiator.Generate();

            _player.Visible = true;
            _player.SetPaused(false);
        }
    }
}
