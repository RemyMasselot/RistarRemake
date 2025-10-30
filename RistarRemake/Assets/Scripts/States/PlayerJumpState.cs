using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerBaseState
{
    public PlayerJumpState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory)
    : base(currentContext, playerStateFactory) { }
    
    //private bool DoCornerCorrection = false;
    //private bool CorrectionLeft = true;
    //private float VelocityUp;
    
    public override void EnterState() 
    {
        //Debug.Log("JUMP ENTER");
        _player.UpdateAnim("Jump");
        _player.Leap = false;
        _player.Rb.gravityScale = 1;
        _player.CoyoteCounter = 0;
        //DoCornerCorrection = false;
        _player.CornerCorrection.enabled = true;
        if (_player.ArmDetection.ObjectDetected == 4)
        {
            Debug.Log("JUMP from star handle");
            Vector2 dir = _player.transform.position - _player.ShCentre;
            
            float percent = (_player._starHandleCurrentValue - 0) / (_player.StarHandleTargetValue - 0) * 100f;
            _player.ShImpulseCurrent = _player.ShImpulseMin + (_player.ShImpulseMax - _player.ShImpulseMin) * (percent / 100f);
            
            _player.Rb.velocity = dir.normalized * _player.ShImpulseCurrent;
            Debug.Log("Dir : " + dir + " velo : " + _player.Rb.velocity);
        }
        else
        {
            _player.Rb.velocity = new Vector2(_player.Rb.velocity.x, _player.JumpForceV);
        }
        _player.ArmDetection.ObjectDetected = 0;
        StartTimer();

        // CAMERA BEHAVIOR
        //_ctx.MainCameraBehavior.CamJumpEnter();
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
            _player.Rb.velocity = new Vector2(moveValue * _player.JumpForceH, _player.Rb.velocity.y);
        }
        else
        {
            _player.Rb.velocity = new Vector2(_player.Rb.velocity.x, _player.Rb.velocity.y);
        }

        //Debug.Log(_ctx.Rb.velocity.y);
        // Si on est a l'apex et que la touche est maintenu, on reste à l'apex
        if (_player.Rb.velocity.y < 0)
        {
            _player.Rb.velocity = new Vector2(_player.Rb.velocity.x, 0);
        }

        if (_player.UseSpine == false)
        {
            // Rotation visuelle -- SANS SPINE
            if (_player.Rb.velocity.x > 0)
            {
                _player.SpriteRenderer.flipX = false;
            }
            if (_player.Rb.velocity.x < 0)
            {
                _player.SpriteRenderer.flipX = true;
            }
        }
        else
        {
            // Rotation visuelle -- AVEC SPINE
            if (_player.Rb.velocity.x > 0)
            {
                _player.SkeletonAnimation.skeleton.ScaleX = 1;
            }
            if (_player.Rb.velocity.x < 0)
            {
                _player.SkeletonAnimation.skeleton.ScaleX = -1;
            }
        }
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
    public override void OnCollisionEnter2D(Collision2D collision) 
    {
        //if (collision.collider != null)
        //{
        //    Vector2 origin = _ctx.transform.position;
        //    Vector2 direction = Vector2.up;
        //    float distance = 0.08f;
        //    int layerToIgnore = LayerMask.NameToLayer("Player");
        //    int mask = ~(1 << layerToIgnore);
        //    VelocityUp = _ctx.Rb.velocity.y;
        //
        //    Debug.Log("Touché : " + collision.collider.name);
        //    if (collision.collider.gameObject.layer == 9)
        //    {
        //        Vector2 endPoint = origin + direction.normalized * 0.6f;
        //        Vector2 newDir;
        //        if (_ctx.transform.position.x < collision.collider.gameObject.transform.position.x)
        //        {
        //            newDir = Vector2.left;
        //            CorrectionLeft = true;
        //        }
        //        else
        //        {
        //            newDir = Vector2.right;
        //            CorrectionLeft = false;
        //        }
        //        Vector2 newOrigin = endPoint + newDir * distance;
        //        Collider2D collider = Physics2D.OverlapPoint(newOrigin);
        //        if (collider != null)
        //        {
        //            Debug.Log("Un objet est présent au point : " + newOrigin + " : " + collider.name);
        //        }
        //        else
        //        {
        //            Debug.Log("Lancer Corner Correction");
        //            DoCornerCorrection = true;
        //        }
        //    }
        //}
    }

    public override void OnCollisionStay2D(Collision2D collision) 
    {
        // je lance un raycast pour savoir si la collision est au dessus de ma tete
        // Si oui, je lance un raycast a partir du point de fin de mon premier raycast
        // Si au point de fin de mon 2eme raycast ce trouve du vide alors je décale mon perso


        if (collision.gameObject.CompareTag("LadderV"))
        {
            if (_player.LadderVDetectionL.IsLadderVDectectedL == LayerMask.NameToLayer("LadderV") || _player.LadderVDetectionR.IsLadderVDectectedR == LayerMask.NameToLayer("LadderV"))
            {
                _player.IsCurrentLadderHorizontal = false;
                _player.Animator.SetFloat("WallVH", 0);
                _player.CornerCorrection.enabled = false;
                SwitchState(_factory.WallClimb());
            }
        }
        else if (collision.gameObject.CompareTag("LadderH"))
        {
            if (_player.LadderHDetection.IsLadderHDectected == true)
            {
                _player.IsCurrentLadderHorizontal = true;
                _player.Animator.SetFloat("WallVH", 1);
                _player.CornerCorrection.enabled = false;
                SwitchState(_factory.WallClimb());
            }
        }
    }
}
