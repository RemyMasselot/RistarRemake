using DG.Tweening;
using UnityEngine;

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

        DOTween.Kill(_player.IkArmLeft);
        DOTween.Kill(_player.IkArmRight);

        Vector2 dir = (new Vector3 (_player.ArmDetection.SnapPosHand.x, _player.ArmDetection.SnapPosHand.y, 0)) - _player.transform.position;
        _player.PlayerRigidbody.velocity = dir.normalized * _player.HeadbuttMoveSpead;
    }
    public override void UpdateState()
    {
        // Move Left Arm
        _player.IkArmLeft.transform.position = _player.ArmDetection.SnapPosHandL;
        // Move Right Arm
        _player.IkArmRight.transform.position = _player.ArmDetection.SnapPosHandR;
    }
    public override void FixedUpdateState() {
    
        if (_player.EnemyDetection.IsGroundDectected == true)
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