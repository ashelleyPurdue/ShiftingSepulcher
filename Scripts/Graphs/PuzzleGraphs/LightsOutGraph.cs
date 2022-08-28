using System;
using System.Collections.Generic;
using RandomDungeons.MathUtils;

namespace RandomDungeons.PuzzleGraphs
{
    public class LightsOutGraph
    {
        public readonly int Width;
        public readonly int Height;

        private bool[,] _lightStates;

        public static LightsOutGraph Generate(
            int seed,
            int width,
            int height,
            int numFlips
        )
        {
            // Start with a solved puzzle
            var graph = new LightsOutGraph(width, height);

            // Scramble the puzzle by activating random lights.
            // This is guaranteed to be solvable, because activating a light
            // is reversible.  The player can just perform this sequence of
            // activations in reverse, and it will return to its original
            // (solved) state.
            var rng = new Random(seed);
            for (int i = 0; i < numFlips; i++)
            {
                var coords = new Vector2i(
                    rng.Next(0, width),
                    rng.Next(0, height)
                );

                graph.ActivateLight(coords);
            }

            return graph;
        }

        public LightsOutGraph(int width, int height)
        {
            Width = width;
            Height = height;

            _lightStates = new bool[width, height];
        }

        public bool IsLightOn(Vector2i coords)
        {
            return _lightStates[coords.x, coords.y];
        }

        public void SetLightOn(Vector2i coords, bool value)
        {
            _lightStates[coords.x, coords.y] = value;
        }

        public void ActivateLight(Vector2i coords)
        {
            // Flip the selected light
            SetLightOn(coords, !IsLightOn(coords));

            // Flip all its neighbors
            foreach (Vector2i neighbor in NeighboringCoordinates(coords))
            {
                SetLightOn(neighbor, !IsLightOn(neighbor));
            }
        }

        /// <summary>
        /// The puzzle is considered solved if all lights are "on".
        /// Hmmm...the name "lights out" sounds backwards...
        /// </summary>
        /// <returns></returns>
        public bool IsSolved()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (!IsLightOn(new Vector2i(x, y)))
                        return false;
                }
            }

            return true;
        }

        private IEnumerable<Vector2i> NeighboringCoordinates(Vector2i coords)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var offset = new Vector2i(i - 1, j - 1);
                    var result = coords + offset;

                    // Skip the original coords, since a point is not its own
                    // neighbor
                    if (result == coords)
                        continue;

                    // Skip any coords that are out-of-bounds
                    if (!InBounds(result))
                        continue;

                    yield return result;
                }
            }
        }

        private bool InBounds(Vector2i point)
        {
            return
                point.x >= 0 &&
                point.y >= 0 &&
                point.x < Width &&
                point.y < Height;
        }
    }
}
