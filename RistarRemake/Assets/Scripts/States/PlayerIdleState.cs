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

        if (_player.IsGrabing == false)
        {
            // Passage en state WALK
            if (_player.MoveH.ReadValue<float>() != 0)
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
                //SwitchState(_factory.Grab());
                _player.StartGrab();
            }

            // Passage en state HEADBUTT ou HANG
            if (_player.GrabScript.NewStateFromGrab != null)
            {
                SwitchState(_player.GrabScript.NewStateFromGrab);
            }
        }
    }

    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnCollisionStay2D(Collision2D collision) { }

}
