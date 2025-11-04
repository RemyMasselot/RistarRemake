using UnityEngine;
using static PlayerStateMachine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    
    public override void EnterState() 
    {
        //Debug.Log("JUMP ENTER");
        //if (_player.UseSpine == false)
        //{
        //    _player.PlayerVisual.UpdateAnim("Jump");
        //}
        _player.PlayerRigidbody.gravityScale = 1;
        _player.CoyoteCounter = 0;
        //DoCornerCorrection = false;
        _player.CornerCorrection.enabled = true;
        if (_player.ArmDetection.ObjectDetected == 4)
        {
            //Debug.Log("JUMP from star handle");
            Vector2 dir = _player.transform.position - _player.StarHandleCentre;
            
            float percent = (_player.StarHandleCurrentValue - 0) / (_player.StarHandleTargetValue - 0) * 100f;
            _player.StarHandleCurrentImpulse = _player.StarHandleImpulseMin + (_player.StarHandleImpulseMax - _player.StarHandleImpulseMin) * (percent / 100f);
            
            _player.PlayerRigidbody.velocity = dir.normalized * _player.StarHandleCurrentImpulse;
            //Debug.Log("Dir : " + dir + " velo : " + _player.PlayerRigidbody.velocity);
        }
        else
        {
            _player.PlayerRigidbody.velocity = new Vector2(_player.PlayerRigidbody.velocity.x, _player.JumpForceV);
        }
        _player.ArmDetection.ObjectDetected = 0;
        StartTimer();
    }
    void StartTimer()
    {
        _player.CurrentTimerValueJump = _player.MaxTimeJump;
        _player.IsTimerRunningJump = true;
    }

    public override void UpdateState() 
    {
        if (_player.IsTimerRunningJump == true)
        {
            _player.CurrentTimerValueJump -= Time.deltaTime;
            if (_player.CurrentTimerValueJump <= 0f)
            {
                _player.IsTimerRunningJump = false;
            }
        }

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

        //Debug.Log(_ctx.Rb.velocity.y);
        // Si on est a l'apex et que la touche est maintenu, on reste à l'apex
        if (_player.PlayerRigidbody.velocity.y < 0)
        {
            _player.PlayerRigidbody.velocity = new Vector2(_player.PlayerRigidbody.velocity.x, 0);
        }

        _player.PlayerDirectionVerif();
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
        if (_player.IsTimerRunningJump == false)
        {
            _player.CornerCorrection.enabled = false;
            SwitchState(_factory.Fall());
        }
        if (_player.Jump.WasReleasedThisFrame())
        {
            _player.IsTimerRunningJump = false;
            _player.CornerCorrection.enabled = false;
            SwitchState(_factory.Fall());
        }

        // Passage en state GRAB
        if (_player.Grab.WasPerformedThisFrame())
        {
            _player.CornerCorrection.enabled = false;
            SwitchState(_factory.Grab());
        }
    }
    public override void OnCollisionEnter2D(Collision2D collision) { }

    public override void OnCollisionStay2D(Collision2D collision) 
    {
        // je lance un raycast pour savoir si la collision est au dessus de ma tete
        // Si oui, je lance un raycast a partir du point de fin de mon premier raycast
        // Si au point de fin de mon 2eme raycast ce trouve du vide alors je décale mon perso

        _player.LadderVerif(collision);

        if (_player.IsLadder != (int)LadderIs.Nothing)
        {
            _player.CornerCorrection.enabled = false;
            SwitchState(_factory.WallClimb());
        }

        //if (_player.IsLadder == (int)LadderIs.VerticalLeft || _player.IsLadder == (int)LadderIs.VerticalRight)
        //{
        //    //_player.IsCurrentLadderHorizontal = false;
        //    _player.Animator.SetFloat("WallVH", 0);
        //    _player.CornerCorrection.enabled = false;
        //    SwitchState(_factory.WallClimb());
        //}
        //else if (_player.IsLadder == (int)LadderIs.Horizontal)
        //{
        //    //if (_player.LadderHDetection.IsLadderHDectected == true)
        //    {
        //        //_player.IsCurrentLadderHorizontal = true;
        //        _player.Animator.SetFloat("WallVH", 1);
        //        _player.CornerCorrection.enabled = false;
        //        SwitchState(_factory.WallClimb());
        //    }
        //}
    }
}
