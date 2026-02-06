using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnockBackState : IEntityState
{
    private readonly Player _player;
    private WaitForSeconds _knockBackDuration = new WaitForSeconds(1f);
    public PlayerKnockBackState(Player player) => _player = player;
    public void OnEnter()
    {
        _player.LockState(true);

        _player.Animator.speed = 1f;
        _player.Animator.SetInteger("AttackIndex", 0);
        _player.Animator.SetBool("IsKnockBack", true);

        _player.StartCoroutine(KnockBackTimer());
    }
    public void OnUpdate() { }
    public void OnFixedUpdate() { }
    public void OnExit()
    {

    }
    private IEnumerator KnockBackTimer()
    {
        yield return _knockBackDuration;

        _player.LockState(false);

        if (_player.gameObject.TryGetComponent<PlayerHpMp>(out var player))
        {
            if(player.CurrentHP <= new BigNumber(0)) _player.ChangeState(_player.DeadState);
            else _player.ChangeState(_player.IdleState);
        }
    }
}
