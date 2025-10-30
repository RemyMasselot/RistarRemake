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
        _ctx.UpdateAnim("Jump");
        _ctx.Leap = false;
        _ctx.Rb.gravityScale = 1;
        _ctx.CoyoteCounter = 0;
        //DoCornerCorrection = false;
        _ctx.CornerCorrection.enabled = true;
        if (_ctx.ArmDetection.ObjectDetected == 4)
        {
            Debug.Log("JUMP from star handle");
            Vector2 dir = _ctx.transform.position - _ctx.ShCentre;
            
            float percent = (_ctx._starHandleCurrentValue - 0) / (_ctx.StarHandleTargetValue - 0) * 100f;
            _ctx.ShImpulseCurrent = _ctx.ShImpulseMin + (_ctx.ShImpulseMax - _ctx.ShImpulseMin) * (percent / 100f);
            
            _ctx.Rb.velocity = dir.normalized * _ctx.ShImpulseCurrent;
            Debug.Log("Dir : " + dir + " velo : " + _ctx.Rb.velocity);
        }
        else
        {
            _ctx.Rb.velocity = new Vector2(_ctx.Rb.velocity.x, _ctx.JumpForceV);
        }
        _ctx.ArmDetection.ObjectDetected = 0;
        StartTimer();

        // CAMERA BEHAVIOR
        //_ctx.MainCameraBehavior.CamJumpEnter();
    }
    void StartTimer()
    {
        _ctx.CurrentTimerValueJump = _ctx.MaxTimeJump;
        _ctx.IsTimerRunningJump = true;
    }

    public override void UpdateState() 
    {
        if (_ctx.IsTimerRunningJump == true)
        {
            _ctx.CurrentTimerValueJump -= Time.deltaTime;
            if (_ctx.CurrentTimerValueJump <= 0f)
            {
                _ctx.IsTimerRunningJump = false;
            }
        }

        CheckSwitchStates();
    }
    public override void FixedUpdateState() 
    {
        float moveValue = _ctx.MoveH.ReadValue<float>();
        if (moveValue != 0)
        {
            _ctx.Rb.velocity = new Vector2(moveValue * _ctx.JumpForceH, _ctx.Rb.velocity.y);
        }
        else
        {
            _ctx.Rb.velocity = new Vector2(_ctx.Rb.velocity.x, _ctx.Rb.velocity.y);
        }

        //Debug.Log(_ctx.Rb.velocity.y);
        // Si on est a l'apex et que la touche est maintenu, on reste à l'apex
        if (_ctx.Rb.velocity.y < 0)
        {
            _ctx.Rb.velocity = new Vector2(_ctx.Rb.velocity.x, 0);
        }

        if (_ctx.UseSpine == false)
        {
            // Rotation visuelle -- SANS SPINE
            if (_ctx.Rb.velocity.x > 0)
            {
                _ctx.SpriteRenderer.flipX = false;
            }
            if (_ctx.Rb.velocity.x < 0)
            {
                _ctx.SpriteRenderer.flipX = true;
            }
        }
        else
        {
            // Rotation visuelle -- AVEC SPINE
            if (_ctx.Rb.velocity.x > 0)
            {
                _ctx.SkeletonAnimation.skeleton.ScaleX = 1;
            }
            if (_ctx.Rb.velocity.x < 0)
            {
                _ctx.SkeletonAnimation.skeleton.ScaleX = -1;
            }
        }
    }
    public override void ExitState() { }
    public override void InitializeSubState() { }
    public override void CheckSwitchStates() 
    {
        // Enter DAMAGE STATE
        if (_ctx.Invincinbility.IsInvincible == false)
        {
            if (_ctx.EnemyDetection.IsGroundDectected == true)
            {
                SwitchState(_factory.Damage());
            }
        }

        // Passage en state FALL
        if (_ctx.IsTimerRunningJump == false)
        {
            _ctx.CornerCorrection.enabled = false;
            SwitchState(_factory.Fall());
        }
        if (_ctx.Jump.WasReleasedThisFrame())
        {
            _ctx.IsTimerRunningJump = false;
            _ctx.CornerCorrection.enabled = false;
            SwitchState(_factory.Fall());
        }

        // Passage en state GRAB
        if (_ctx.Grab.WasPerformedThisFrame())
        {
            _ctx.CornerCorrection.enabled = false;
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
            if (_ctx.LadderVDetectionL.IsLadderVDectectedL == LayerMask.NameToLayer("LadderV") || _ctx.LadderVDetectionR.IsLadderVDectectedR == LayerMask.NameToLayer("LadderV"))
            {
                _ctx.Animator.SetFloat("WallVH", 0);
                _ctx.CornerCorrection.enabled = false;
                SwitchState(_factory.WallClimb());
            }
        }
        if (collision.gameObject.CompareTag("LadderH"))
        {
            if (_ctx.LadderHDetection.IsLadderHDectected == true)
            {
                _ctx.Animator.SetFloat("WallVH", 1);
                _ctx.CornerCorrection.enabled = false;
                SwitchState(_factory.WallClimb());
            }
        }
    }
}
