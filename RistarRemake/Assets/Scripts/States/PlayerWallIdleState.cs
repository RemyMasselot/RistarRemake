using UnityEngine;
using static PlayerStateMachine;

public class PlayerWallIdleState : PlayerBaseState
{
    public PlayerWallIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    
    public override void EnterState()
    {
        //Debug.Log("ENTER WALL IDLE");
        _player.UpdateAnim("WallIdle");
        _player.PlayerRigidbody.velocity = Vector2.zero;
        _player.PlayerRigidbody.gravityScale = 0;
        if (_player.UseSpine == false)
        {
            if (_player.Animator.GetFloat("WallVH") == 0)
            {
                //Debug.Log("WALL VERTICAL");
                if (_player.LadderVDetectionL.IsLadderVDectectedL == true)
                {
                    _player.IsPlayerTurnToLeft = true;
                    Debug.Log("LEFT");
                }
                if (_player.LadderVDetectionR.IsLadderVDectectedR == true)
                {
                    _player.IsPlayerTurnToLeft = false;
                    Debug.Log("RIGHT");
                }
            }
        }
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void FixedUpdateState(){ }
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

        // Passage en state WALL CLIMB
        if (_player.UseSpine == false)
        {
            if (_player.IsLadder == (int)LadderIs.VerticalLeft || _player.IsLadder == (int)LadderIs.VerticalRight) //Echelle Vertical
            {
                float moveValueV = _player.MoveV.ReadValue<float>();
                if (Mathf.Abs(moveValueV) != 0)
                {
                    SwitchState(_factory.WallClimb());
                }
                if (_player.Jump.WasPerformedThisFrame())
                {
                    if (Mathf.Abs(moveValueV) > 0) // Passage en state WALL JUMP
                    {
                        SwitchState(_factory.WallJump());
                    }
                    else // Passage en state FALL
                    {
                        _player.IsPlayerTurnToLeft = !_player.IsPlayerTurnToLeft;
                        SwitchState(_factory.Fall());
                    }
                }
            }
            else if (_player.IsLadder == (int)LadderIs.Horizontal) //Echelle Horizontal
            {
                float moveValueH = _player.MoveH.ReadValue<float>();
                if (Mathf.Abs(moveValueH) != 0)
                {
                    SwitchState(_factory.WallClimb());
                }
                if (_player.Jump.WasPerformedThisFrame())
                {
                    SwitchState(_factory.Fall());
                }
            }
        }
    }

    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnCollisionStay2D(Collision2D collision) { }

}