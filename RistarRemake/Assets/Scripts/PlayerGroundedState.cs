using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState
{
    public override void EnterState() {
        Debug.Log("GROUNDED STATE");
    }
    public override void UpdateState() { }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() { }
}
