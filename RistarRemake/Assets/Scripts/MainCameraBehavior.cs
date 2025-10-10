using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
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
    public float PosFallY;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float rayLength = 10f;
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

        // MOVE DOWN
        //CorrectPosY();
        //DOTween.To(() => CameraPositionFallOff.y, x => CameraPositionFallOff.y = x, newYDown, 1f);
        //if (rbPlayer.velocity.y < 0)
        //{
        //    //DOTween.KillAll();
        //    //If proche du sol, on limite la nouvelle pos Y
        //}
        //else if (rbPlayer.velocity.y > 0)
        //{
        //    DOTween.To(() => CameraPositionFallOff.y, x => CameraPositionFallOff.y = x, PosWallUpY, 1f);
        //}

        // NEW POSITION
        if (CameraImpacted == false)
        {
            if (CameraInde == true)
            {
                NewTarget = target.transform.position + CameraPositionDefault + CameraPositionFallOff + aimV3;
            }
            if (NewTarget.y <= -1)
            {
                transform.DOMove(new Vector3(NewTarget.x, -1, -1), 0.5f);
            }
            else
            {
                transform.DOMove(new Vector3(NewTarget.x, NewTarget.y, -1), 0.5f);
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

    public void CorrectPosY(Vector2 PosToVerif)
    {
        Vector2 origin = playerStateMachine.gameObject.transform.position;
        Vector2 direction = Vector2.down;

        RaycastHit2D hitPlayer = Physics2D.Raycast(origin, direction, rayLength, groundLayer);
        RaycastHit2D hitCam = Physics2D.Raycast(PosToVerif, direction, rayLength, groundLayer);

        if (hitPlayer.collider == hitCam.collider)
        {
            Debug.DrawRay(origin, direction * hitPlayer.distance, Color.green); // Vert si collision
            float distance = Mathf.Abs(Vector2.Distance(new Vector2(0, hitPlayer.point.y), new Vector2(0, transform.position.y)));
            if (distance < 4.2f)
            {
                newYDown = 0;
            }
            else
            {
                newYDown = PosWallDownY;
            }
        }
    }

    public void CorrectPosY()
    {
        Vector2 origin = playerStateMachine.gameObject.transform.position;
        Vector2 direction = Vector2.down;

        RaycastHit2D hit = Physics2D.Raycast(origin, direction, rayLength, groundLayer);

        if (hit.collider != null)
        {
            Debug.DrawRay(origin, direction * hit.distance, Color.green); // Vert si collision
            float distance = Mathf.Abs(Vector2.Distance(new Vector2(0, hit.point.y), new Vector2(0, transform.position.y)));
            //Debug.Log(distance);
            if (distance < 4.2f)
            {
                newYDown = 0;
            }
            else
            {
                newYDown = PosWallDownY;
            }
            //if (transform.position.y <= hit.point.y + 3.2f)
            //{
            //    //transform.position = new Vector3(transform.position.x, hit.point.y + 2.2f, transform.position.z);
            //    newYDown = 0;
            //    Debug.Log("correction Y");
            //    return;
            //}
            //else
            //{
            //    //newYDown = PosWallDownY;
            //    return;
            //}
        }
        else
        {
            Debug.DrawRay(origin, direction * rayLength, Color.red); // Rouge si rien
            //newYDown = PosWallDownY;
            //return; // Rien détecté -> retourne 0
        }
    }
}
