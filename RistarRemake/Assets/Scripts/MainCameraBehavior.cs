using DG.Tweening;
using UnityEngine;

public class MainCameraBehavior : MonoBehaviour
{
    [Header ("SETTINGS")]
    [SerializeField] private PlayerStateMachine playerStateMachine;
    public GameObject target;
    public Camera Camera;
    public Vector3 CameraPositionDefault;
    public Vector3 CameraPositionFallOff;
    [HideInInspector] public Vector3 NewTarget;
    [HideInInspector] public string CurrentState;


    //// VARIABLE DATA
    [Header ("DEFAULT")]
    public float SizeDefault;

    [Header ("AIM")]
    public float AimMultiplier;

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

    private Vector3 currentPosition;

    private void Start()
    {
        target = playerStateMachine.gameObject;
        playerStateMachine.CameraInde = true;

        currentPosition = playerStateMachine.transform.position;
    }

    private void LateUpdate()
    {
        Vector3 targetPosition = playerStateMachine.transform.position;
        float lerpSpeed = 5f;

        if (!playerStateMachine.CameraInde)
        {
            targetPosition = playerStateMachine.CameraTargetOverride;
        }

        if (playerStateMachine.CurrentState is PlayerMeteorStrikeState)
        {
            targetPosition += (Vector3)playerStateMachine.PlayerRigidbody.velocity.normalized * 2f;
            lerpSpeed = 15f;
        }

        currentPosition = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime * lerpSpeed);
        currentPosition.z = -10f;
        transform.position = currentPosition;
    }
    
    public void CamJumpEnter()
    {
        DOTween.KillAll();
        Camera.DOOrthoSize(SizeJump, 0.2f);
        DOTween.To(() => CameraPositionFallOff.y, x => CameraPositionFallOff.y = x, PosJumpY, 0.2f);
    }
}
