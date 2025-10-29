using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MainCameraBehavior : MonoBehaviour
{
    [Header ("SETTINGS")]
    public GameObject target;
    [SerializeField] private PlayerStateMachine playerStateMachine;
    private Rigidbody2D rbPlayer;
    public Camera Camera;
    public Vector3 CameraPositionDefault;
    public Vector3 CameraPositionFallOff;
    [HideInInspector] public bool CameraInde;
    [HideInInspector] public bool CameraImpacted;
    [HideInInspector] public Vector3 NewTarget;
    [HideInInspector] public string CurrentState;


    //// VARIABLE DATA
    [Header ("DEFAULT")]
    public float SizeDefault;

    [Header ("AIM")]
    public float AimMultiplier;
    private float aimX = 0;
    private float aimY = 0;
    private Vector3 aimV3;

    [Header ("WALK")]
    public float PosDirectionX;
    public float PosWalkX;

    [Header ("JUMP")]
    public float SizeJump;
    public float PosJumpY;

    [Header ("FALL")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float rayLength = 10f;
    public float PosFallY;
    public float newYDown;

    [Header ("WALL")]
    public float PosWallUpY;
    public float PosWallDownY;

    [Header("HEADBUTT")]
    public Vector3 CamShakeHeabbutt;


    private void Start()
    {
        target = playerStateMachine.gameObject;
        CameraImpacted = false;
        CameraInde = true;
        rbPlayer = playerStateMachine.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // DIRECTION
        if (playerStateMachine.SpriteRenderer.flipX == false)
        {
            DOTween.To(() => CameraPositionFallOff.x, x => CameraPositionFallOff.x = x, PosDirectionX, 2f);
        }
        else
        {
            DOTween.To(() => CameraPositionFallOff.x, x => CameraPositionFallOff.x = x, -PosDirectionX, 2f);
        }

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

        //CLIMB
        if (CurrentState == "CLIMB")
        {
            if (GroundVerif() > 2)
            {
                float climbValue = playerStateMachine.MoveV.ReadValue<float>();
                if (climbValue > 0)
                {
                    DOTween.To(() => CameraPositionFallOff.y, x => CameraPositionFallOff.y = x, PosWallUpY, 0.5f);
                    //Debug.Log("CLIMB UP");
                }
                else if (climbValue < 0)
                {
                    DOTween.To(() => CameraPositionFallOff.y, x => CameraPositionFallOff.y = x, PosWallDownY, 0.5f);
                    //Debug.Log("CLIMB DOWN");
                }
            }
            else
            {
                DOTween.To(() => CameraPositionFallOff.y, x => CameraPositionFallOff.y = x, 0, 0.5f);
            }
        }

        // FALL
        if (CurrentState == "FALL")
        {
            if (GroundVerif() > 2)
            {
                float inputValue = playerStateMachine.MoveV.ReadValue<float>();
                if (inputValue < 0)
                {
                    float fallValue = PosFallY + inputValue * newYDown;
                    DOTween.To(() => CameraPositionFallOff.y, x => CameraPositionFallOff.y = x, fallValue, 0.8f);
                }
                else
                {
                    DOTween.To(() => CameraPositionFallOff.y, x => CameraPositionFallOff.y = x, PosFallY, 0.8f);
                }
            }
            else
            {
                DOTween.To(() => CameraPositionFallOff.y, x => CameraPositionFallOff.y = x, 0, 0.8f);
            }
        }

        // NEW POSITION
        if (CameraImpacted == false)
        {
            if (CameraInde == true)
            {
                NewTarget = target.transform.position + CameraPositionDefault + CameraPositionFallOff;
            }
            
            if (GroundVerif() <= 1)
            {
                transform.DOMove(new Vector3(NewTarget.x, transform.position.y, -1) + aimV3, 0.5f);
            }
            else
            {
                transform.DOMove(new Vector3(NewTarget.x, NewTarget.y, -1) + aimV3, 0.5f);
            }
        }

        
    }

    public void PlayerTouchGround()
    {
        DOTween.KillAll();
        Camera.DOOrthoSize(SizeDefault, 0.8f);
        DOTween.To(() => CameraPositionFallOff.y, x => CameraPositionFallOff.y = x, 0, 0.8f);
    }
    
    public void CamJumpEnter()
    {
        DOTween.KillAll();
        Camera.DOOrthoSize(SizeJump, 0.2f);
        DOTween.To(() => CameraPositionFallOff.y, x => CameraPositionFallOff.y = x, PosJumpY, 0.2f);
    }

    //public void CorrectPosY(Vector2 PosToVerif)
    //{
    //    Vector2 origin = playerStateMachine.gameObject.transform.position;
    //    Vector2 direction = Vector2.down;

    //    RaycastHit2D hitPlayer = Physics2D.Raycast(origin, direction, rayLength, groundLayer);
    //    RaycastHit2D hitCam = Physics2D.Raycast(PosToVerif, direction, rayLength, groundLayer);

    //    if (hitPlayer.collider == hitCam.collider)
    //    {
    //        Debug.DrawRay(origin, direction * hitPlayer.distance, Color.green); // Vert si collision
    //        float distance = Mathf.Abs(Vector2.Distance(new Vector2(0, hitPlayer.point.y), new Vector2(0, transform.position.y)));
    //        if (distance < 4.2f)
    //        {
    //            newYDown = 0;
    //        }
    //        else
    //        {
    //            newYDown = PosWallDownY;
    //        }
    //    }
    //}

    public float GroundVerif()
    {
        Vector2 origin = playerStateMachine.gameObject.transform.position;
        Vector2 direction = Vector2.down;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, rayLength, groundLayer);

        if (hit.collider != null)
        {
            float distance = hit.distance;
            Debug.DrawRay(origin, direction * hit.distance, Color.green);
            Debug.Log("Distance sol : " + distance);
            return distance;
        }
        else
        {
            Debug.DrawRay(origin, direction * rayLength, Color.red);
            return rayLength;
        }
    }
}
