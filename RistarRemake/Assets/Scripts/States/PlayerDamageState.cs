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
        _ctx.LifeNumber--;
        if (_ctx.LifeNumber > 0)
        {
            _ctx.Rb.gravityScale = 1;
            _ctx.Rb.velocity = new Vector2(0, _ctx.LeapForceV/1.5f);
            _ctx.Invincinbility.InvincibilityCounter = _ctx.Invincinbility.InvincibilityTime;
            _ctx.Invincinbility.IsInvincible = true;
        }
    }
    public override void UpdateState() 
    {
        CheckSwitchStates();
    }
    public override void FixedUpdateState() 
    {
        float moveValue = _ctx.MoveH.ReadValue<float>();
        if (moveValue != 0)
        {
            _ctx.Rb.velocity = new Vector2(moveValue * _ctx.JumpForceH, _ctx.Rb.velocity.y);
        }
        else
        {
            _ctx.Rb.velocity = new Vector2(_ctx.Rb.velocity.x, _ctx.Rb.velocity.y);
        }

        // Rotation visuelle -- SANS SPINE
        if (_ctx.Rb.velocity.x > 0)
        {
            _ctx.SpriteRenderer.flipX = false;
        }
        if (_ctx.Rb.velocity.x < 0)
        {
            _ctx.SpriteRenderer.flipX = true;
        }
    }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() 
    {
        // Passage en state DEATH
        if (_ctx.LifeNumber <= 0)
        {
            SwitchState(_factory.Death());
        }
        
        // Passage en state FALL
        if (_ctx.Rb.velocity.y < 0)
        {
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
