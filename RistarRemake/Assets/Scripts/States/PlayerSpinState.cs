using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpinState : PlayerBaseState
{
    public PlayerSpinState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }


    public override void EnterState()
    {
        Debug.Log("ENTER SPIN");
        _ctx.UpdateAnim("Spin");
        if (_ctx.UseSpine == false)
        {
            _ctx.Arms.gameObject.SetActive(false);
        }
        _ctx.Rb.gravityScale = 1;
        if (_ctx.AimDir.magnitude > 0)
        {
            _ctx.Rb.velocity = new Vector2(1, 1) * 5;
        }
        else
        {
            _ctx.Rb.velocity = new Vector2(-1, 1) * 5;
        }
    }
    public override void UpdateState()
    {
        // Passage en state FALL
        if (_ctx.Rb.velocity.y < 0)
        {
            SwitchState(_factory.Fall());
        }
    }
    public override void FixedUpdateState() { }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() { }
    public override void OnCollision(Collision2D collision) { }
}