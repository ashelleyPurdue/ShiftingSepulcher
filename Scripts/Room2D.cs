using System;
using System.Linq;
using Godot;

namespace RandomDungeons
{
    [CustomNode(parent: "Node2D", icon:"Room")]
    public class Room2D : Node2D
    {
        public RoomEntrance GetEntrance(string name)
        {
            return this.AllDescendantsOfType<RoomEntrance>()
                .First(e => e.Name == name);
        }
    }
}
