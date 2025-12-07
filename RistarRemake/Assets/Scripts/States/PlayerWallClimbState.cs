using UnityEngine;
using static PlayerStateMachine;

public class PlayerWallClimbState : PlayerBaseState
{
    public PlayerWallClimbState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    
    public override void EnterState()
    {
        //Debug.Log("ENTER WALL CLIMB");

        //_player.PlayerDirectionVerif();
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void FixedUpdateState()
    {
        PlayerMovement();
    }
    private void PlayerMovement()
    {
        if (_player.IsLadder == (int)LadderIs.VerticalLeft || _player.IsLadder == (int)LadderIs.VerticalRight)
        {
            float moveValueV = _player.MoveV.ReadValue<float>();
            if (moveValueV > 0)
            {
                _player.PlayerRigidbody.velocity = new Vector2(0, _player.WalkSpeed * Time.deltaTime);
            }
            if (moveValueV < 0)
            {
                _player.PlayerRigidbody.velocity = new Vector2(0, -_player.WalkSpeed * Time.deltaTime);
            }
        }
        else if (_player.IsLadder == (int)LadderIs.Horizontal)
        {
            float moveValueH = _player.MoveH.ReadValue<float>();
            if (moveValueH > 0)
            {
                _player.IsPlayerTurnToLeft = false;
                _player.PlayerRigidbody.velocity = new Vector2(_player.WalkSpeed * Time.deltaTime, 0);
            }
            if (moveValueH < 0)
            {
                _player.IsPlayerTurnToLeft = true;
                _player.PlayerRigidbody.velocity = new Vector2(-_player.WalkSpeed * Time.deltaTime, 0);
            }
        }
    }

    public override void ExitState(){}
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

        if (_player.IsLadder == (int)LadderIs.VerticalLeft || _player.IsLadder == (int)LadderIs.VerticalRight) // VERTICAL
        {
            // Passage en state WALL IDLE
            float moveValueV = _player.MoveV.ReadValue<float>();
            if (moveValueV == 0)
            {
                SwitchState(_factory.WallIdle());
            }
            else if (moveValueV > 0)
            {
                // Passage en state JUMP
                if (_player.Jump.WasPerformedThisFrame())
                {
                    SwitchState(_factory.Jump());
                }
            }
            else if (moveValueV < 0)
            {
                // Passage en state FALL
                if (_player.Jump.WasPerformedThisFrame())
                {
                    SwitchState(_factory.Fall());
                }
            }
        }
        else if (_player.IsLadder == (int)LadderIs.Horizontal) // HORIZONTAL
        {
            // Passage en state WALL IDLE
            float moveValueH = _player.MoveH.ReadValue<float>();
            if (Mathf.Abs(moveValueH) == 0)
            {
                SwitchState(_factory.WallIdle());
            }
            else if (_player.Jump.WasPerformedThisFrame())
            {
                SwitchState(_factory.Fall());
            }
        }


    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        _player.LadderVerif(collision);
    }

    public override void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.GetComponent<LadderExitDetection>() != null)
        {
            ChoseExitToTake(collision.GetComponent<LadderExitDetection>());
        }
    }

    private void ChoseExitToTake(LadderExitDetection ladderExitDetection)
    {
        float moveValueV = _player.MoveV.ReadValue<float>();
        float moveValueH = _player.MoveH.ReadValue<float>();

        if (ladderExitDetection.TriggerIdIs == LadderExitDetection.TriggerId.TriggerUp)
        {
            if (moveValueV > 0f)
            {
                SwitchState(_factory.Leap());
            }
        }
        else if (ladderExitDetection.TriggerIdIs == LadderExitDetection.TriggerId.TriggerDown)
        {
            if (moveValueV < 0f)
            {
                SwitchState(_factory.Fall());
            }
        }
        else if (ladderExitDetection.TriggerIdIs == LadderExitDetection.TriggerId.TriggerLeft)
        {
            if (moveValueH < 0f)
            {
                SwitchState(_factory.Fall());
            }
        }
        else if (ladderExitDetection.TriggerIdIs == LadderExitDetection.TriggerId.TriggerRight)
        {
            if (moveValueH > 0f)
            {
                SwitchState(_factory.Fall());
            }
        }
    }
}