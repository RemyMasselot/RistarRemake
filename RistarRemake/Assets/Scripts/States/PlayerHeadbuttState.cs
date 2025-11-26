using DG.Tweening;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

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

        _player.HeadbuttDirection = (new Vector3 (_player.ArmDetection.SnapPosHand.x, _player.ArmDetection.SnapPosHand.y, 0)) - _player.transform.position;
        _player.PlayerRigidbody.velocity = _player.HeadbuttDirection.normalized * _player.HeadbuttMoveSpead;
    }
    public override void UpdateState()
    {
        // Move Left Arm
        _player.IkArmLeft.transform.position = _player.ArmDetection.SnapPosHand;
        // Move Right Arm
        _player.IkArmRight.transform.position = _player.ArmDetection.SnapPosHand;
    }
    public override void FixedUpdateState() {
    
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
        if (collision.gameObject.CompareTag("Wall"))
        {
            SwitchState(_factory.Spin());
        }
    }
}