using System.Collections;
using UnityEngine;

public class PlayerDeadState : IEntityState
{
    private readonly Player _player;
    private WaitForSeconds _deadDuration = new WaitForSeconds(1f);
    public PlayerDeadState(Player player) => _player = player;
    public void OnEnter() 
    {
        _player.LockState(true);

        _player.Animator.speed = 1f;
        _player.Animator.SetInteger("AttackIndex", 0);
        _player.Animator.SetBool("IsKnockBack", false);
        _player.Animator.SetBool("IsDead", true);
        _player.Animator.SetBool("IsSkilling", false);

        Player.TriggerDead();

        _player.StartCoroutine(WaitResurrectionCo());
    }
    public void OnUpdate() { }
    public void OnFixedUpdate() { }
    public void OnExit() 
    {

    }
    private IEnumerator WaitResurrectionCo()
    {
        _player.GetComponent<Collider2D>().enabled = false;
        yield return _deadDuration;

        // 플레이어 초기화
    }
}
