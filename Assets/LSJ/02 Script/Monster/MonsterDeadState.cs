using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDeadState : IEntityState
{
    private readonly MonsterBase _monster;

    public MonsterDeadState(MonsterBase monster) => _monster = monster;
    public void OnEnter() 
    {
        _monster.LockState(true);

        _monster.Animator.SetBool("IsAttacking",false);
        _monster.Animator.SetBool("IsDead",true);

        _monster.GetComponent<Collider2D>().enabled = false;
    }
    public void OnUpdate() { }
    public void OnFixedUpdate() { }
    public void OnExit() { }
}
