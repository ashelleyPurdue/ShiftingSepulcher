using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace RandomDungeons
{
    [Tool]
    public class DungeonTreeTemplateRoom : Node
    {
        [Export] public ChallengeType ChallengeType;
        [Export] public int KeyId;
        [Export] public int LockId;
        [Export] public NodePath[] Shortcuts = new NodePath[0];
    }
}
