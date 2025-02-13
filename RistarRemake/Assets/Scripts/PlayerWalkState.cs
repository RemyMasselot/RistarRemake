using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerWalkState : PlayerBaseState
{
    public PlayerWalkState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    public override void EnterState() { }
    public override void UpdateState() { }
    public override void FixedUpdateState() {
        // Déplacements du personnage
        float moveValue = _ctx.Walk.ReadValue<float>();
        _ctx.Transform.position += new Vector3(moveValue * _ctx.WalkSpeed * Time.deltaTime, 0, 0);
    }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() { }
}
