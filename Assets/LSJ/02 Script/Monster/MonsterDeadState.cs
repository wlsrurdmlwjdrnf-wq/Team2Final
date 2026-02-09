using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDeadState : IEntityState
{
    private readonly MonsterBase _monster;

    public MonsterDeadState(MonsterBase monster) => _monster = monster;
    public void OnEnter() 
    {
        _monster.Animator.SetBool("IsAttacking",false);
        _monster.Animator.SetBool("IsDead",true);
    }
    public void OnUpdate() { }
    public void OnFixedUpdate() { }
    public void OnExit() { }
}
