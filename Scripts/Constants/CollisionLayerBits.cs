using Godot;

namespace RandomDungeons
{
    public enum CollisionLayerBits
    {
        Walls = 1,
        StopsEnemiesOnly = 2,
        StopsIceBlocksOnly = 4
    }
}
