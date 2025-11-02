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
        _player.UpdateAnim("Damage");
        _player.LifeNumber--;
        if (_player.LifeNumber > 0)
        {
            _player.PlayerRigidbody.gravityScale = 1;
            _player.PlayerRigidbody.velocity = new Vector2(0, _player.LeapForceV/1.5f);
            _player.Invincinbility.InvincibilityCounter = _player.Invincinbility.InvincibilityTime;
            _player.Invincinbility.IsInvincible = true;
        }
    }
    public override void UpdateState() 
    {
        CheckSwitchStates();
    }
    public override void FixedUpdateState() 
    {
        float moveValue = _player.MoveH.ReadValue<float>();
        if (moveValue != 0)
        {
            _player.PlayerRigidbody.velocity = new Vector2(moveValue * _player.JumpForceH, _player.PlayerRigidbody.velocity.y);
        }
        else
        {
            _player.PlayerRigidbody.velocity = new Vector2(_player.PlayerRigidbody.velocity.x, _player.PlayerRigidbody.velocity.y);
        }

        // Rotation visuelle -- SANS SPINE
        //if (_player.Rb.velocity.x > 0)
        //{
        //    _player.SpriteRenderer.flipX = false;
        //}
        //if (_player.Rb.velocity.x < 0)
        //{
        //    _player.SpriteRenderer.flipX = true;
        //}
    }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() 
    {
        // Passage en state DEATH
        if (_player.LifeNumber <= 0)
        {
            SwitchState(_factory.Death());
        }
        
        // Passage en state FALL
        if (_player.PlayerRigidbody.velocity.y < 0)
        {
            SwitchState(_factory.Fall());
        }

        // Passage en state GRAB
        if (_player.Grab.WasPerformedThisFrame())
        {
            SwitchState(_factory.Grab());
        }
    }
    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnCollisionStay2D(Collision2D collision) { }
}
