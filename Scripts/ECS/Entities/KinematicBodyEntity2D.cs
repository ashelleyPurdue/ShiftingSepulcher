using Godot;

namespace RandomDungeons
{
    public class KinematicBodyEntity2D : KinematicBody2D, IEntity<KinematicBody2D>
    {
        public KinematicBody2D Node => this;

        public override void _Ready()
        {
            this.SendEntityReady();
        }
    }
}
