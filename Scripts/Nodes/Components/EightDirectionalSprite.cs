using System;
using Godot;
using RandomDungeons.Utils;

namespace RandomDungeons.Nodes.Components
{
    // [Tool]
    public class EightDirectionalSprite : Node2D
    {
        [Export] public EightDirection Direction;
        [Export] public string Animation = "default";
        [Export] public float SpeedScale = 1;
        [Export] public bool Playing;

        [Export] public SpriteFrames North;
        [Export] public SpriteFrames NorthEast;
        [Export] public SpriteFrames East;
        [Export] public SpriteFrames SouthEast;
        [Export] public SpriteFrames South;
        [Export] public SpriteFrames SouthWest;
        [Export] public SpriteFrames West;
        [Export] public SpriteFrames NorthWest;

        public SpriteFrames GetSpriteFrames(EightDirection dir)
        {
            switch (dir)
            {
                case EightDirection.North: return North;
                case EightDirection.NorthEast: return NorthEast;
                case EightDirection.East: return East;
                case EightDirection.SouthEast: return SouthEast;
                case EightDirection.South: return South;
                case EightDirection.SouthWest: return SouthWest;
                case EightDirection.West: return West;
                case EightDirection.NorthWest: return NorthWest;

                // Since this script also runs in the editor, it's important for
                // it to not crash.  Hence, we're using default instead of
                // throwing an exception.
                default: return South;
            }
        }

        public override void _Process(float delta)
        {
            Node spriteNode = GetNode("Sprite");
            if (!(spriteNode is AnimatedSprite _sprite))
                return;

            _sprite.Animation = Animation;
            _sprite.SpeedScale = SpeedScale;
            _sprite.Playing = Playing;

            // Changing _sprite.Frames resets the animation, so we only want
            // to do that if the direction has actually _changed_.
            if (_sprite.Frames != GetSpriteFrames(Direction))
                _sprite.Frames = GetSpriteFrames(Direction);
        }
    }
}
