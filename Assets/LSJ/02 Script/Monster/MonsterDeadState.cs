using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDeadState : IEntityState
{
    private readonly MonsterBase _monster;
    private WaitForSeconds _deathTime = new WaitForSeconds(1);

    public MonsterDeadState(MonsterBase monster) => _monster = monster;
    public void OnEnter() 
    {
        _monster.Animator.SetBool("IsAttacking",false);
        _monster.Animator.SetBool("IsDead",true);
        
        if(_monster.gameObject.activeSelf)
            _monster.StartCoroutine(ShowDeadCo());
    }
    public void OnUpdate() { }
    public void OnFixedUpdate() { }
    public void OnExit() { }

    private IEnumerator ShowDeadCo()
    {
        _monster.Collider.enabled = false;
        yield return _deathTime;
        PoolManager2.Instance.Release(_monster.gameObject);
    }
}
