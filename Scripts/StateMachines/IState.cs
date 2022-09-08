namespace RandomDungeons.StateMachines
{
    public interface IState
    {
        IStateMachine StateMachine {get; set;}
        object Owner {get; set;}

        void _Process(float delta);
        void _PhysicsProcess(float delta);

        void _StateEntered();
        void _StateExited();
    }
}
