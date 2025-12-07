using DG.Tweening;
using UnityEngine;
using static PlayerStateMachine;

public class PlayerHeadbuttState : PlayerBaseState
{
    public PlayerHeadbuttState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }


    public override void EnterState()
    {
        //Debug.Log("ENTER HEADBUTT");

        //// Move Left Arm
        //_player.IkArmLeft.transform.DOPause();
        //// Move Right Arm
        //_player.IkArmRight.transform.DOPause();

        // CAMERA BEHAVIOR
        _player.CameraTargetOverride = _player.ArmDetection.SnapPosHand;

        _player.GrabScript.NewStateFromGrab = null;

        DOTween.Kill(_player.IkArmLeft);
        DOTween.Kill(_player.IkArmRight);

        _player.HeadbuttDirection = _player.ArmDetection.SnapPosHand - _player.transform.position;
        _player.PlayerRigidbody.velocity = _player.HeadbuttDirection.normalized * _player.HeadbuttMoveSpead;
    }
    public override void UpdateState()
    {
        SetHandPosition();
    }

    private void SetHandPosition()
    {
        // Move Left Arm
        _player.IkArmLeft.transform.position = _player.ArmDetection.SnapPosHandL;
        // Move Right Arm
        _player.IkArmRight.transform.position = _player.ArmDetection.SnapPosHandR;
    }

    public override void FixedUpdateState() 
    {
    
        if (_player.EnemyDetection.IsDectected == true)
        {
            SwitchState(_factory.Spin());
        }
    }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() { }
    public override void OnCollisionEnter2D(Collision2D collision) { }
    public override void OnCollisionStay2D(Collision2D collision)
    {
        // COLLISION WITH WALL
        if (_player.platformCollisionDetection.WallDetected 
            || _player.platformCollisionDetection.CeilingDetected)
        {
            SwitchState(_factory.Spin());
            Debug.Log("HEADBUTT WALL");
        }

        // COLLISION LADDER
        _player.LadderVerif(collision);

        if (_player.IsLadder != (int)LadderIs.Nothing)
        {
            SwitchState(_factory.WallIdle());
        }
    }
}