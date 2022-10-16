using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using RandomDungeons.DungeonTrees;

namespace RandomDungeons.Nodes.TreeTemplates
{
    public class DungeonTreeTemplateRoom : Node
    {
        [Export] public Graphs.ChallengeType ChallengeType;
        [Export] public int KeyId;
        [Export] public int LockId;
        [Export] public NodePath[] Shortcuts = new NodePath[0];
    }
}
