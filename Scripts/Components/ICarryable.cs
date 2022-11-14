using Godot;

namespace RandomDungeons
{
    public interface ICarryable
    {
        /// <summary>
        /// The node that is being carried
        /// </summary>
        /// <value></value>
        Node2D Node {get;}

        bool IsBeingCarried {get;}

        void PickUp(Node2D carrier);
        void Release(Vector2 releasePosGlobal);
        void Throw(Vector2 releasePosGlobal, Vector2 direction);
    }
}
