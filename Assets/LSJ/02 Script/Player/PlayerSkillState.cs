using UnityEngine;

public class PlayerSkillState : IEntityState
{
    private readonly Player _player;
    public PlayerSkillState(Player player) => _player = player;
    public void OnEnter() 
    {
        _player.Animator.speed = 1f;
        _player.Animator.SetBool("IsSkilling", true);
        _player.Animator.SetInteger("AttackIndex", 0);
    }
    public void OnUpdate() { }
    public void OnFixedUpdate() { }
    public void OnExit() { }
}
