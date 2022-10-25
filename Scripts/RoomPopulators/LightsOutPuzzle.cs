using System;
using System.Collections.Generic;
using Godot;

namespace RandomDungeons
{
    public class LightsOutPuzzle : Node2D, IRoomPopulator
    {
        [Export] public PackedScene SwitchPrefab;
        [Export] public Color OnColor;
        [Export] public Color OffColor;

        public event Action Solved;
        private bool _isFrozen = false;

        private LightsOutGraph _graph;

        private Dictionary<Vector2i, LightsOutSwitch> _switches
            = new Dictionary<Vector2i, LightsOutSwitch>();

        public bool IsSolved() => _graph?.IsSolved() ?? true;

        public void Populate(DungeonGraphRoom graphRoom, Random rng)
        {
            var graph = LightsOutGraph.Generate(
                seed: graphRoom.RoomSeed,
                width: 4,
                height: 4,
                numFlips: 3
            );

            SetGraph(graph);
        }

        private void SetGraph(LightsOutGraph graph)
        {
            _graph = graph;

            // Clear out all the switches
            foreach (var s in _switches.Values)
            {
                RemoveChild(s);
                s.QueueFree();
            }
            _switches.Clear();

            // Create new switches to match the graph
            foreach(var coords in AllCoordinates())
            {
                var s = SwitchPrefab.Instance<LightsOutSwitch>();
                s.Position = ToRealPos(coords);
                s.Activated += () => LightActivated(coords);

                AddChild(s);

                _switches[coords] = s;
            }
        }

        public override void _Process(float delta)
        {
            foreach (var coords in AllCoordinates())
            {
                _switches[coords].Modulate = _graph.IsLightOn(coords)
                    ? OnColor
                    : OffColor;
            }
        }

        private void LightActivated(Vector2i coords)
        {
            // Don't respond to input if the puzzle is already solved
            if (_isFrozen)
                return;

            // Activate the light
            _graph.ActivateLight(coords);

            // Fire an event if we just solved the puzzle
            if (_graph.IsSolved())
            {
                _isFrozen = true;
                Solved?.Invoke();
            }
        }

        private IEnumerable<Vector2i> AllCoordinates()
        {
            for (int x = 0; x < _graph.Width; x++)
            {
                for (int y = 0; y < _graph.Height; y++)
                {
                    yield return new Vector2i(x, y);
                }
            }
        }

        private Vector2 ToRealPos(Vector2i puzzlePos)
        {
            var unshifted = new Vector2(puzzlePos.x, puzzlePos.y) * 32 * 2;
            var offset = new Vector2(_graph.Width, _graph.Height) * -32;
            return unshifted + offset;
        }
    }
}
