public interface IState 
{
    string Name { get; }

    public void Tick();

    public void OnEnter();

    public void OnExit();
}
