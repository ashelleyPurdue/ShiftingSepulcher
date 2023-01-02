using Godot;

namespace RandomDungeons
{
    public interface IRoomEntrance
    {
        Node2D Node {get;}
        string EntranceName {get;}
    }
}
