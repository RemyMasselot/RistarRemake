using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeathState : PlayerBaseState
{
    public PlayerDeathState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    
    public override void EnterState() 
    {
        //Debug.Log("ENTER DEATH");
        if (_player.SpriteRenderer.flipX == false)
        {
            _player.UpdateAnim("DeathL");
        }
        else
        {
            _player.UpdateAnim("DeathR");
        }
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
        
    }
    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnCollisionStay2D(Collision2D collision) { }
}
