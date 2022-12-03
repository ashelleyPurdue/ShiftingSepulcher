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

        bool RotatesWhileHeld {get;}

        void PickUp();
        void Release();
        void Throw(Vector2 direction);
    }
}
