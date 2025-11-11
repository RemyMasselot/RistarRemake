using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{
    public PlayerIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    
    public override void EnterState()
    {
        //Debug.Log("ENTER IDLE");
        _player.PlayerRigidbody.velocity = Vector2.zero;
        _player.CoyoteCounter = _player.CoyoteTime;
    }
    public override void UpdateState()
    {
        _player.CountTimePassedInState();
        CheckSwitchStates();
    }
    public override void FixedUpdateState(){}
    public override void ExitState(){}
    public override void InitializeSubState(){}
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

        // Passage en state WALK
        if (_player.MoveH.WasPerformedThisFrame())
        {
            SwitchState(_factory.Walk());
        }

        // Passage en state JUMP
        if (_player.Jump.WasPerformedThisFrame())
        {
            SwitchState(_factory.Jump());
        }
        else if (_player.JumpBufferCounter <= _player.JumpBufferTime)
        {
            _player.LowJumpActivated = true;
            SwitchState(_factory.Jump());
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
