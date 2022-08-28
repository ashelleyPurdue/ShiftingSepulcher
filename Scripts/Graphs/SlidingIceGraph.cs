using System;
using System.Collections.Generic;

using RandomDungeons.Utils;

namespace RandomDungeons.Graphs
{
    public class SlidingIceGraph
    {
        public readonly int Width;
        public readonly int Height;

        public readonly Vector2i StartPos;
        public Vector2i EndPos {get; private set;}

        public IEnumerable<Vector2i> RockPositions => _rockPositions;
        private HashSet<Vector2i> _rockPositions = new HashSet<Vector2i>();

        public static SlidingIceGraph Generate(
            int seed,
            int width,
            int height,
            int numPushes
        )
        {
            // TODO: use The Algorithm (tm) to generate an always-completable
            // puzzle.
            // Right now, we're just randomly throwing things in there without
            // a care.

            var rng = new Random(seed);

            var startPos = new Vector2i(
                rng.Next(0, width),
                rng.Next(0, height)
            );
            var endPos = new Vector2i(
                rng.Next(0, width),
                rng.Next(0, height)
            );

            var graph = new SlidingIceGraph(width, height, startPos);
            graph.EndPos = endPos;

            for (int i = 0; i < numPushes; i++)
            {
                var rockPos = new Vector2i(
                    rng.Next(0, width),
                    rng.Next(0, height)
                );
                graph._rockPositions.Add(rockPos);
            }

            return graph;
        }

        public SlidingIceGraph(int width, int height, Vector2i startPos)
        {
            Width = width;
            Height = height;

            StartPos = startPos;
            EndPos = startPos;
        }
    }
}