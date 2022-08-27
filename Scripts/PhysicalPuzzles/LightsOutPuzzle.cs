using System;
using System.Collections.Generic;
using Godot;

using RandomDungeons.MathUtils;
using RandomDungeons.PuzzleGraphs;

namespace RandomDungeons.PhysicalPuzzles
{
    public class LightsOutPuzzle : Node2D
    {
        [Export] public PackedScene SwitchPrefab;
        [Export] public Color OnColor;
        [Export] public Color OffColor;

        private LightsOutGraph _graph;

        private Dictionary<Vector2i, LightsOutSwitch> _switches
            = new Dictionary<Vector2i, LightsOutSwitch>();

        public void SetGraph(LightsOutGraph graph)
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
                s.Position = new Vector2(coords.x, coords.y) * 32 * 2;
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
    }
}
