using UnityEngine;
using static PlayerStateMachine;

public class PlayerWallIdleState : PlayerBaseState
{
    public PlayerWallIdleState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        //Debug.Log("ENTER WALL IDLE");

        _player.PlayerRigidbody.velocity = Vector2.zero;
        _player.CoyoteCounter = 0;

        _player.PlayerDirectionVerif();

        if (_player.IsGrabing == true)
        {
            _player.GrabScript.ExitGrab();
        }

        if (_player.CanSnapPositionLadder)
        {
            _player.transform.position = _player.LadderSnapPosition;
            _player.CanSnapPositionLadder = false;
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
            if (_player.EnemyDetection.IsDectected == true)
            {
                SwitchState(_factory.Damage());
            }
        }
        
        if (_player.IsLadder == (int)LadderIs.VerticalLeft || _player.IsLadder == (int)LadderIs.VerticalRight) //Echelle Vertical
        {
            float moveValueV = _player.MoveV.ReadValue<float>();
            if (Mathf.Abs(moveValueV) != 0) // Passage en state WALL CLIMB
            {
                SwitchState(_factory.WallClimb()); 
            }
            else if (_player.Jump.WasPerformedThisFrame())
            {
                if (Mathf.Abs(moveValueV) > 0) // Passage en state WALL JUMP
                {
                    Debug.Log(moveValueV);
                    SwitchState(_factory.Jump());
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
            else if (_player.Jump.WasPerformedThisFrame())
            {
                SwitchState(_factory.Fall());
            }
        }
    }

    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnCollisionStay2D(Collision2D collision) { }

}