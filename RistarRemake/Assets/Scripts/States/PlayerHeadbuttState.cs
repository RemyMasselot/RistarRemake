using DG.Tweening;
using UnityEngine;

public class PlayerHeadbuttState : PlayerBaseState
{
    public PlayerHeadbuttState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }


    public override void EnterState()
    {
        Debug.Log("ENTER HEADBUTT");
        //if (_player.UseSpine == false)
        //{
        //    _player.PlayerVisual.UpdateAnim("Headbutt");
        //}
        // Move Left Arm
        _player.IkArmLeft.transform.DOPause();
        //_ctx.IkArmLeft.transform.DOLocalMove(_ctx.DefaultPosLeft.localPosition, _ctx.DurationExtendGrab);
        // Move Right Arm
        _player.IkArmRight.transform.DOPause();
        //_ctx.IkArmRight.transform.DOLocalMove(_ctx.DefaultPosRight.localPosition, _ctx.DurationExtendGrab);
        Vector2 dir = (new Vector3 (_player.ArmDetection.SnapPosHand.x, _player.ArmDetection.SnapPosHand.y, 0)) - _player.transform.position;
        _player.PlayerRigidbody.velocity = dir.normalized * 10;
    }
    public override void UpdateState()
    {
        // Move Left Arm
        _player.IkArmLeft.transform.position = _player.ArmDetection.SnapPosHand;
        // Move Right Arm
        _player.IkArmRight.transform.position = _player.ArmDetection.SnapPosHand;

        if (_player.UseSpine == false)
        {
            // Draw Line Arm
            //_player.LineArmLeft.SetPosition(0, _player.ShoulderLeft.position);
            //_player.LineArmLeft.SetPosition(1, _player.IkArmLeft.position);
            //_player.LineArmRight.SetPosition(0, _player.ShoulderRight.position);
            //_player.LineArmRight.SetPosition(1, _player.IkArmRight.position);
        }
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
    public override void OnCollisionStay2D(Collision2D collision) { }
}