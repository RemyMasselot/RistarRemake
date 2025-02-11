using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBaseState
{
    public abstract void EnterState();
    public abstract void UpdateState();
    public abstract void ExitState();
    public abstract void CheckSwitchStates();
    public abstract void InitializeSubState();

    void UpdateStates() { }
    void SwitchState() { }
    void SetSuperState() { }
    void SetSubState() { }
}
