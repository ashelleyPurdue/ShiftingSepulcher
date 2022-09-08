namespace RandomDungeons.StateMachines
{
    public interface IState
    {
        StateMachine StateMachine {get; set;}
        object Owner {get; set;}

        void _Process(float delta);
        void _PhysicsProcess(float delta);

        void _StateEntered();
        void _StateExited();
    }
}
