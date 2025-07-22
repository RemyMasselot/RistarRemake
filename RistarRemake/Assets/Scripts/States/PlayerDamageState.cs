using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageState : PlayerBaseState
{
    public PlayerDamageState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    
    public override void EnterState() 
    {
        //Debug.Log("ENTER DAMAGE");
        _ctx.UpdateAnim("Damage");
        
    }
    public override void UpdateState() 
    {
        
    }
    public override void FixedUpdateState() 
    {
        
    }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() 
    {
        // Passage en state FALL
        if (_ctx.IsTimerRunningJump == false)
        {
            _ctx.CornerCorrection.enabled = false;
            SwitchState(_factory.Fall());
        }

        // Passage en state GRAB
        if (_ctx.Grab.WasPerformedThisFrame())
        {
            SwitchState(_factory.Grab());
        }
    }
    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnCollisionStay2D(Collision2D collision) { }
}
