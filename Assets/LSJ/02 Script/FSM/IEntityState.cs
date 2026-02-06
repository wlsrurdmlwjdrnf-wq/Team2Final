public interface IEntityState
{
    public void OnEnter();
    public void OnUpdate();
    public void OnFixedUpdate();
    public void OnExit();
}
