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

        public IEnumerable<Vector2i> BlockPositions => _blockPositions;
        private HashSet<Vector2i> _blockPositions = new HashSet<Vector2i>();

        public SlidingIceGraph(int width, int height, Vector2i startPos)
        {
            Width = width;
            Height = height;

            StartPos = startPos;
            EndPos = startPos;
        }
    }
}