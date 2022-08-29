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

        private void Push(Vector2i dir, int dist)
        {
            // Slide the goal in some direction, as if we were pushing the ice
            // block.
            EndPos += dir * dist;

            // Place a rock in the tile right after it, to stop the imaginary
            // ice block
            _rockPositions.Add(EndPos + dir);
        }

        /// <summary>
        /// Returns the maximum number of tiles an imaginary ice block, starting
        /// at <paramref name="pos"/>, can be pushed in direction
        /// <paramref name="dir"/>, before it hits a rock or the edge of the
        /// board.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        private int MaxPushDistance(Vector2i pos, Vector2i dir)
        {
            int dist = 0;

            Vector2i nextPos = pos + dir;
            while (IsInBounds(nextPos) && !IsRock(nextPos))
            {
                dist++;
                pos += dir;
                nextPos += dir;
            }

            return dist;
        }

        private bool IsInBounds(Vector2i pos)
        {
            return
                (pos.x >= 0 && pos.y >= 0) &&
                (pos.x < Width && pos.y < Height);
        }

        private bool IsRock(Vector2i pos)
        {
            return _rockPositions.Contains(pos);
        }
    }
}
