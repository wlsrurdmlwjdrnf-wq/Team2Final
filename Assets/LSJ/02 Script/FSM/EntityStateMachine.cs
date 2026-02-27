using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityStateMachine : MonoBehaviour
{
    protected IEntityState currentState;
    protected bool _isLocked = false;

    protected virtual void Update()
    {
        currentState?.OnUpdate();
    }
    protected virtual void FixedUpdate()
    {
        currentState?.OnFixedUpdate();
    }
    public virtual void ChangeState(IEntityState newState)
    {
        if (_isLocked) return; // true : 상태전환잠금

        if (currentState == newState)
        {
            currentState?.OnEnter(); // 재진입시 OnEnter만 실행
            return;
        }

        currentState?.OnExit();
        currentState = newState;
        currentState?.OnEnter();
    }
    public void LockState(bool locked)
    {
        _isLocked = locked;
    }
}
