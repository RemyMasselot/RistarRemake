using UnityEngine;
using static ArmDetection;

public class PlayerSpinState : PlayerBaseState
{
    public PlayerSpinState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        //Debug.Log("ENTER SPIN");

        _player.CoyoteCounter = 0;
        _player.GrabScript.NewStateFromGrab = null;

        // Move Left Arm
        _player.IkArmLeft.transform.position = _player.DefaultPosLeft.position;
        // Move Right Arm
        _player.IkArmRight.transform.position = _player.DefaultPosRight.position;

        _player.transform.position = _player.ArmDetection.SnapPosHand;

        _player.PlayerCollider.enabled = false;

        SpinMovement();
    }
    private void SpinMovement()
    {
        if (_player.ArmDetection.ObjectGrabed == (int)ObjectGrabedIs.Wall)
        {
            if (_player.LadderHDetection.IsLadderHDectected == true)
            {
                _player.PlayerRigidbody.velocity = new Vector2(0, -1);
                Debug.Log("LADDER SPIN");
            }
            else
            {
                _player.PlayerRigidbody.velocity = _player.IsPlayerTurnToLeft ? new Vector2(1, 1) * 4 : new Vector2(-1, 1) * 4;
                //Debug.Log("WALL SPIN");
            }
        }
        else if (_player.ArmDetection.ObjectGrabed == (int)ObjectGrabedIs.Ceiling)
        {
            _player.PlayerRigidbody.velocity = new Vector2(0, -1);
        }
        else if (_player.ArmDetection.ObjectGrabed == (int)ObjectGrabedIs.Enemy)
        {
            _player.PlayerRigidbody.velocity = new Vector2(0, 1) * 10;
            _player.ElementGrabed.GetComponent<Enemy>().BeKilled();
        }
        else if (_player.ArmDetection.ObjectGrabed == (int)ObjectGrabedIs.Floor)
        {
            _player.PlayerRigidbody.velocity = new Vector2(0, 1);
        }
        //Debug.Log(_player.ArmDetection.ObjectDetected);
    }

    public override void UpdateState()
    {
        _player.CountTimePassedInState();

        CheckSwitchStates();
    }

    public override void FixedUpdateState() { }

    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() 
    {
        if (_player.TimePassedInState >= _player.SpinTime)
        {
            //Debug.Log(_player.TimePassedInState);
            //_player.PlayerRigidbody.velocity = new Vector2(-1, _player.PlayerRigidbody.velocity.y);
            _player.PlayerCollider.enabled = true;
            SwitchState(_factory.Fall());
        }
    }
    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnCollisionStay2D(Collision2D collision) { }
}