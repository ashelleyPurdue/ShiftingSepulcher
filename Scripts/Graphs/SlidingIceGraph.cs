using System;
using System.Collections.Generic;
using System.Linq;

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
        private HashSet<Vector2i> _criticalPathPositions = new HashSet<Vector2i>();

        public static SlidingIceGraph Generate(
            int seed,
            int width,
            int height,
            int numPushes
        )
        {
            var rng = new Random(seed);

            var startPos = new Vector2i(
                rng.Next(0, width),
                rng.Next(0, height)
            );

            var graph = new SlidingIceGraph(width, height, startPos);

            for (int i = 0; i < numPushes; i++)
            {
                // Pick a random pushable direction.
                // If no directions are pushable, then stop the puzzle early.
                Vector2i[] pushableDirs = graph
                    .PushableDirs(graph.EndPos)
                    .ToArray();

                if (pushableDirs.Length == 0)
                    break;

                Vector2i dir = rng.PickFrom(pushableDirs);

                // Pick a random distance to push it.
                var legalDists = graph.LegalPushDistances(graph.EndPos, dir);
                int dist = rng.PickFrom(legalDists);

                graph.Push(dir, dist);
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
            // Slide the goal in the given direction, as if we were pushing the
            // ice block.
            for (int i = 0; i < dist; i++)
            {
                EndPos += dir;
                _criticalPathPositions.Add(EndPos);
            }

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

        /// <summary>
        /// Returns a list of all distances that an imaginary block in position
        /// <paramref name="pos"/> could be pushed in direction
        /// <paramref name="dir"/>, without the puzzle becoming impossible.
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="dir"></param>
        /// <returns></returns>
        private IEnumerable<int> LegalPushDistances(Vector2i pos, Vector2i dir)
        {
            int maxDist = MaxPushDistance(pos, dir);
            for (int i = 0; i < maxDist; i++)
            {
                if (!_criticalPathPositions.Contains(pos))
                    yield return (i + 1);
            }
        }

        /// <summary>
        /// Returns all the directions an imaginary block in the given position
        /// can be pushed
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        private IEnumerable<Vector2i> PushableDirs(Vector2i pos)
        {
            var allDirs = new Vector2i[]
            {
                Vector2i.Up,
                Vector2i.Down,
                Vector2i.Left,
                Vector2i.Right
            };

            return allDirs
                .Where(d => LegalPushDistances(pos, d).Any());
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
