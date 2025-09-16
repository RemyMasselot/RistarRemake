using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Core;
using Spine;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class MainCameraBehavior : MonoBehaviour
{
    // SETTINGS
    public GameObject target;
    [SerializeField] private PlayerStateMachine playerStateMachine;
    [SerializeField] private Camera Camera;
    public Vector3 CameraPositionDefault;
    public Vector3 CameraPositionFallOff;

    private TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> tweeningSizeIdle;


    //// VARIABLE DATA
    // DEFAULT
    public float SizeDefault;

    // AIM
    public float AimMultiplier;
    private float aimX = 0;
    private float aimY = 0;
    private Vector3 aimV3;

    // WALK
    public float PosWalkX;

    // JUMP
    public float SizeJump;
    public float PosJumpY;

    // FALL
    public float PosFallY;

    private void Start()
    {
        target = playerStateMachine.gameObject;
    }

    void Update()
    {
        // WALK
        float moveValue = playerStateMachine.MoveH.ReadValue<float>();
        if (moveValue > 0)
        {
            DOTween.To(() => CameraPositionFallOff.x, x => CameraPositionFallOff.x = x, PosWalkX, 2f);
        }
        else if(moveValue < 0)
        {
            DOTween.To(() => CameraPositionFallOff.x, x => CameraPositionFallOff.x = x, -PosWalkX, 2f);
        }


        // AIM
        Vector2 aimValue = playerStateMachine.Aim.ReadValue<Vector2>();

        DOTween.To(() => aimX, x => aimX = x, aimValue.x, 1f);
        DOTween.To(() => aimY, x => aimY = x, aimValue.y, 1f);
        
        aimV3 = new Vector3(aimX, aimY, 0);


        // NEW POSITION
        transform.position = target.transform.position + CameraPositionDefault + CameraPositionFallOff + aimV3;
    }

    public void CamIdleEnter()
    {
        Camera.DOOrthoSize(SizeDefault, 0.8f);
        tweeningSizeIdle = DOTween.To(() => CameraPositionFallOff.y, x => CameraPositionFallOff.y = x, 0, 0.8f);
    }
    
    public void CamJumpEnter()
    {
        DOTween.KillAll();
        Camera.DOOrthoSize(SizeJump, 0.2f);
        DOTween.To(() => CameraPositionFallOff.y, x => CameraPositionFallOff.y = x, PosJumpY, 0.2f);
    }
}
