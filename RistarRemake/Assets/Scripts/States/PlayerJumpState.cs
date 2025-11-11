using UnityEngine;
using static PlayerStateMachine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    
    public override void EnterState() 
    {
        //Debug.Log("JUMP ENTER");

        _player.PlayerRigidbody.gravityScale = 1;
        _player.CoyoteCounter = 0;

        if (_player.ArmDetection.ObjectDetected == 4)
        {
            //Debug.Log("JUMP from star handle");
            Vector2 dir = _player.transform.position - _player.StarHandleCentre;
            
            float percent = (_player.StarHandleCurrentValue - 0) / (_player.StarHandleTargetValue - 0) * 100f;
            _player.StarHandleCurrentImpulse = _player.StarHandleImpulseMin + (_player.StarHandleImpulseMax - _player.StarHandleImpulseMin) * (percent / 100f);
            
            _player.PlayerRigidbody.velocity = dir.normalized * _player.StarHandleCurrentImpulse;
        }
        else
        {
            _player.PlayerRigidbody.velocity = new Vector2(_player.PlayerRigidbody.velocity.x, _player.JumpForceV);
        }
        _player.ArmDetection.ObjectDetected = 0;
    }

    public override void UpdateState() 
    {
        _player.CountTimePassedInState();

        _player.PlayerDirectionVerif();

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

        // Si on est a l'apex et que la touche est maintenu, on reste à l'apex
        if (_player.PlayerRigidbody.velocity.y < 0)
        {
            _player.PlayerRigidbody.velocity = new Vector2(_player.PlayerRigidbody.velocity.x, 0);
        }

    }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() 
    {
        // Enter DAMAGE STATE
        if (_player.Invincinbility.IsInvincible == false)
        {
            if (_player.EnemyDetection.IsGroundDectected == true)
            {
                SwitchState(_factory.Damage());
            }
        }

        // Passage en state FALL
        if (_player.TimePassedInState >= _player.MaxTimeJump)
        {
            SwitchState(_factory.Fall());
        }
        else if (_player.TimePassedInState >= 0.1f)
        {
            if (_player.Jump.WasReleasedThisFrame())
            {
                SwitchState(_factory.Fall());
            }
        }

        // Passage en state GRAB
        if (_player.Grab.WasPerformedThisFrame())
        {
            SwitchState(_factory.Grab());
        }
    }
    public override void OnCollisionEnter2D(Collision2D collision) { }

    public override void OnCollisionStay2D(Collision2D collision) 
    {
        _player.LadderVerif(collision);

        if (_player.IsLadder != (int)LadderIs.Nothing)
        {
            SwitchState(_factory.WallClimb());
        }
    }
}
