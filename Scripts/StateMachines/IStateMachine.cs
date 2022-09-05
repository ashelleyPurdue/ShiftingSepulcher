namespace RandomDungeons.StateMachines
{
    public interface IStateMachine
    {
        void ChangeState(IState state);
    }
}
