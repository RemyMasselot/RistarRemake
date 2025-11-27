using UnityEngine;
using static ArmDetection;

public class PlayerSpinState : PlayerBaseState
{
    public PlayerSpinState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }

    public override void EnterState()
    {
        //Debug.Log("ENTER SPIN");

        //_player.PlayerRigidbody.gravityScale = 1;
        //_player.ArmDetection.gameObject.SetActive(false);

        _player.GrabScript.NewStateFromGrab = null;

        _player.transform.position = _player.ArmDetection.SnapPosHand;

        SpinMovement();

        // Move Left Arm
        _player.IkArmLeft.transform.position = _player.DefaultPosLeft.position;
        // Move Right Arm
        _player.IkArmRight.transform.position = _player.DefaultPosRight.position;
    }
    private void SpinMovement()
    {
        if (_player.ArmDetection.ObjectDetected == (int)ObjectDetectedIs.Wall)
        {
            if (_player.LadderHDetection.IsLadderHDectected == true)
            {
                _player.PlayerRigidbody.velocity = new Vector2(0, -1);
            }
            else
            {
                _player.PlayerRigidbody.velocity = _player.IsPlayerTurnToLeft ? new Vector2(1, 1) * 4 : new Vector2(-1, 1) * 4;
                Debug.Log("WALL SPIN");
            }
        }
        else if (_player.ArmDetection.ObjectDetected == (int)ObjectDetectedIs.Enemy)
        {
            _player.PlayerRigidbody.velocity = new Vector2(0, 1) * 10;
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
            SwitchState(_factory.Fall());
        }
    }
    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnCollisionStay2D(Collision2D collision) { }
}