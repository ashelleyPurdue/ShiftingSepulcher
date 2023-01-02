using System;
using System.Linq;
using Godot;

namespace RandomDungeons
{
    [CustomNode(parent: "Node2D", icon:"Room")]
    public class Room2D : Node2D
    {
        public IRoomEntrance GetEntrance(string name)
        {
            return this.AllDescendantsOfType<IRoomEntrance>()
                .First(e => e.EntranceName == name);
        }
    }
}
