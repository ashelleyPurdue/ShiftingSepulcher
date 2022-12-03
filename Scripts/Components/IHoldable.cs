using Godot;

namespace RandomDungeons
{
    public interface IHoldable
    {
        /// <summary>
        /// The node that is being held
        /// </summary>
        /// <value></value>
        Node2D Node {get;}

        bool IsBeingHeld {get;}

        void PickUp(Node2D carrier);
        void Release(Vector2 releasePosGlobal);
        void Throw(Vector2 releasePosGlobal, Vector2 direction);
    }
}
