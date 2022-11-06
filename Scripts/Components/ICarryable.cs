using Godot;

namespace RandomDungeons
{
    public interface ICarryable
    {
        /// <summary>
        /// The node that is being carried
        /// </summary>
        /// <value></value>
        Node Node {get;}

        void PickUp();
        void Release();
    }
}
