using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    #region VARIABLES

    // CAMERA
    [HideInInspector] public Vector3 CameraTargetOverride;
    
    // STATES
    public PlayerStateFactory StatesFactory;
    public PlayerBaseState CurrentState;
    public PlayerBaseState PreviousState;

    [FoldoutGroup("INPUT ACTIONS")] private Controller controls;
    [FoldoutGroup("INPUT ACTIONS")] public InputAction MoveH;
    [FoldoutGroup("INPUT ACTIONS")] public InputAction MoveV;
    [FoldoutGroup("INPUT ACTIONS")] public InputAction Jump;
    [FoldoutGroup("INPUT ACTIONS")] public InputAction Grab;
    [FoldoutGroup("INPUT ACTIONS")] public InputAction Aim;
    [FoldoutGroup("INPUT ACTIONS")] public InputAction Back;

    [FoldoutGroup("REFERENCES")]
    [FoldoutGroup("REFERENCES/Grab")] public Grab GrabScript;
    [FoldoutGroup("REFERENCES/Grab")] public ArmDetection ArmDetection;
    [FoldoutGroup("REFERENCES/Grab")] public Transform IkArmRight;
    [FoldoutGroup("REFERENCES/Grab")] public Transform IkArmLeft;
    [FoldoutGroup("REFERENCES/Grab")] public Transform ShoulderRight;
    [FoldoutGroup("REFERENCES/Grab")] public Transform ShoulderLeft;
    [FoldoutGroup("REFERENCES/Grab")] public Transform DefaultPosRight;
    [FoldoutGroup("REFERENCES/Grab")] public Transform DefaultPosLeft;

    [field: SerializeField, FoldoutGroup("REFERENCES/Detections")] public EnemyDetection EnemyDetection { get; private set; }
    [field: SerializeField, FoldoutGroup("REFERENCES/Detections")] public GroundDetection GroundDetection { get; private set; }
    [field: SerializeField, FoldoutGroup("REFERENCES/Detections")] public LadderVDetectionL LadderVDetectionL { get; private set; }
    [field: SerializeField, FoldoutGroup("REFERENCES/Detections")] public LadderVDetectionR LadderVDetectionR { get; private set; }
    [field: SerializeField, FoldoutGroup("REFERENCES/Detections")] public LadderHDetection LadderHDetection { get; private set; }
    [field: SerializeField, FoldoutGroup("REFERENCES/Detections")] public PlatformCollisionDetection platformCollisionDetection { get; set; }

    [FoldoutGroup("GENERAL SETTING")] public int LifesNumber = 4;
    [FoldoutGroup("GENERAL SETTING")] public int InvicibilityTime = 2;
    [HideInInspector] public UnityEvent NewStatePlayed;
    [HideInInspector] public bool IsPlayerTurnToLeft = false;
    [HideInInspector] public float TimePassedInState = 0;
    [HideInInspector] public Invincinbility Invincinbility;
    [HideInInspector] public CornerCorrection CornerCorrection;
    private Rigidbody2D _playerRigidbody;
    public Rigidbody2D PlayerRigidbody 
    { get 
        { 
            if (_playerRigidbody == null) 
            { 
                _playerRigidbody = GetComponent<Rigidbody2D>(); 
            } 
            return _playerRigidbody; 
        }
    }
    private Collider2D _playerCollider;
    public Collider2D PlayerCollider 
    { get 
        { 
            if (_playerCollider == null) 
            {
                _playerCollider = GetComponent<Collider2D>(); 
            } 
            return _playerCollider; 
        }
    }

    [FoldoutGroup("WALK")] public float WalkMinSpeed = 4;
    [FoldoutGroup("WALK")] public float WalkMaxSpeed = 10;

    [HideInInspector] public Vector2 LadderSnapPosition;
    [HideInInspector] public bool CanSnapPositionLadder = true;
    [HideInInspector] public Collider2D ColliderLadder;
    public enum LadderIs
    {
        Nothing = 0,
        VerticalLeft = 1,
        VerticalRight = 2,
        Horizontal = 3
    }
    [HideInInspector] public int IsLadder = (int)LadderIs.Nothing;

    [FoldoutGroup("JUMP")] public float VerticalJumpDistanceHigh = 1.7f;
    [FoldoutGroup("JUMP")] public float VerticalJumpDistanceLow = 1.4f;
    [FoldoutGroup("JUMP")] public float MaxTimeAtApex = 0.25f;
    [FoldoutGroup("JUMP")] public float TimeToGoToApex = 0.5f;
    [FoldoutGroup("JUMP")] public float MaxSpeedToGoToApex = 8f;
    [FoldoutGroup("JUMP")] public AnimationCurve JumpSpeedCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [FoldoutGroup("JUMP")] public float HorizontalJumpMovementMultiplier = 4f;
    [FoldoutGroup("JUMP")] public float JumpBufferTime = 0.1f;
    [HideInInspector] public float JumpBufferCounter;
    [HideInInspector] public bool LowJumpActivated;
    [FoldoutGroup("JUMP")] public float CoyoteTime = 0.1f;
    [HideInInspector] public float CoyoteCounter;

    [FoldoutGroup("FALL")] public float FallSpeedMax = 15f;
    [FoldoutGroup("FALL")] public float TimeToGoToFallSpeedMax = 0.8f;
    [FoldoutGroup("FALL")] public AnimationCurve FallSpeedCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [FoldoutGroup("FALL")] public float InputFallSpeedIncrease = -3f;
    [FoldoutGroup("FALL")] public float InputFallSpeedDecrease = 0f;

    [FoldoutGroup("LEAP")] public AnimationCurve LeapCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [FoldoutGroup("LEAP")] public float TimeToLeap = 0.7f;
    [FoldoutGroup("LEAP")] public float LeapForce = 2f;

    [HideInInspector] public bool StartGrabInitialisation = false;
    [HideInInspector] public bool IsGrabing = false;
    [HideInInspector] public Vector2 AimDir;
    [FoldoutGroup("GRAB")] public float GrabBufferTime = 0.1f;
    [HideInInspector] public float GrabBufferCounter = 10;
    [FoldoutGroup("GRAB")] public float DistanceGrab = 0.8f;
    [FoldoutGroup("GRAB")] public float TimeToExtendArms = 0.2f;
    [FoldoutGroup("GRAB")] public float MaxHoldGrabTime = 1;
    [FoldoutGroup("GRAB")] public float MaxTimeStayingAtApex = 0.3f;
    [FoldoutGroup("GRAB")] public float FallInGrabValue = -6;
    [FoldoutGroup("GRAB")] public float HorizontalGrabMovementMultiplier = 1.5f;
    [FoldoutGroup("GRAB")] public float VerticalGrabMovementMultiplier = 1;

    [HideInInspector] public Vector2 HeadbuttDirection;
    [FoldoutGroup("HEADBUTT"), Range(0f, 0.1f)] public float HeadbuttMinDuration;
    [FoldoutGroup("HEADBUTT"), Range(0.1f, 0.5f)] public float HeadbuttMaxDuration;
    [FoldoutGroup("HEADBUTT")] public AnimationCurve HeadbuttAccelerationCurve;
    [FoldoutGroup("HEADBUTT")] public float HeadbuttDistanceForTimeMax;

    [FoldoutGroup("SPIN")] public float SpinTime;

    [HideInInspector] public Vector3 StarHandleCentre;
    [FoldoutGroup("STAR HANDLE")] public float TimeToChargeMeteorStrike = 3;
    [HideInInspector] public float StarHandleCurrentValue = 0;
    [FoldoutGroup("STAR HANDLE")] public float StarHandleTargetValue = 200;
    [FoldoutGroup("STAR HANDLE")] public float StarHandleRayonMin = 1.5f;
    [FoldoutGroup("STAR HANDLE")] public float StarHandleRayonMax = 2.5f;
    [HideInInspector] public float StarHandleCurrentRayon = 1.5f;
    [FoldoutGroup("STAR HANDLE")] public float StarHandleMinSpeed = 6;
    [FoldoutGroup("STAR HANDLE")] public float StarHandleMaxSpeed = 12;
    [HideInInspector] public float StarHandleCurrentSpeed = 6;
    [FoldoutGroup("STAR HANDLE")] public float StarHandleSpeedSlowMotion = 8;
    [HideInInspector] public float StarHandleCurrentImpulse = 0;
    [FoldoutGroup("STAR HANDLE")] public float StarHandleImpulseMin = 10;
    [FoldoutGroup("STAR HANDLE")] public float StarHandleImpulseMax = 30;
    
    [FoldoutGroup("METEOR STRIKE")] public float MeteorSpeed = 10;
    [FoldoutGroup("METEOR STRIKE")] public float MaxTimeMeteor = 10;
    [HideInInspector] public bool IsTimerRunningMeteor = true;
    [HideInInspector] public float CurrentTimerValueMeteor = 0;
    [HideInInspector] public Vector2 MeteorStrikeDirection;


    #endregion

    private void Awake()
    {
        // setup state
        StatesFactory = new PlayerStateFactory(this);
        CurrentState = StatesFactory.Idle();
        CurrentState.EnterState();

        Invincinbility = GetComponent<Invincinbility>();
        CornerCorrection = GetComponent<CornerCorrection>();


        if (NewStatePlayed == null)
        {
            NewStatePlayed = new UnityEvent();
        }
        NewStatePlayed.AddListener(ResetTimePassedInState);
    }

    private void Start()
    {
        // Set input actions
        controls = new Controller();
        controls.Enable();
        MoveH = controls.LAND.MOVEH;
        MoveV = controls.LAND.MOVEV;
        Jump = controls.LAND.JUMP;
        Grab = controls.LAND.GRAB;
        Aim = controls.LAND.AIM;
        Back = controls.LAND.BACK;
    }

    void Update()
    {
        CurrentState.UpdateState();
    }

    void FixedUpdate()
    {
        CurrentState.FixedUpdateState();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CurrentState.OnCollisionEnter2D(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        CurrentState.OnCollisionStay2D(collision);
    }
    
    private void OnTriggerStay2D(Collider2D collision)
    {
        CurrentState.OnTriggerStay2D(collision);
    }

    public void PlayerDirectionVerif()
    {
        if (PlayerRigidbody.velocity.x != 0)
        {
            IsPlayerTurnToLeft = PlayerRigidbody.velocity.x < 0;
        }
        else if (IsLadder == (int)LadderIs.VerticalLeft)
        {
            IsPlayerTurnToLeft = true;
        }
        else if (IsLadder == (int)LadderIs.VerticalRight)
        {
            IsPlayerTurnToLeft = false;
        }
    }

    public void LadderVerif()
    {
        IsLadder = (int)LadderIs.Nothing;

        // Cr�er une box detection a gauche, qui renvoie true false
        // Pareil a droite
        // Pareil au dessus du joueur
        // Si �a renvoie true alors le ladder prend la valeur correspondante

        // Param�tres de la box (ajustez les multiplicateurs selon vos besoins)
        float boxWidth = PlayerCollider.bounds.size.x * 0.6f;
        float boxHeight = PlayerCollider.bounds.size.y * 0.6f;
        float sideOffset = PlayerCollider.bounds.extents.x + 0.12f; // distance horizontale pour left/right
        float upOffset = PlayerCollider.bounds.extents.y + 0.12f;   // distance verticale pour top

        LayerMask ladderMask = LayerMask.GetMask("Ladder");

        Vector2 pos = transform.position;

        // Box gauche
        Vector2 leftCenter = pos + Vector2.left * sideOffset;
        Collider2D colliderLeft = Physics2D.OverlapBox(leftCenter, new Vector2(boxWidth, boxHeight), 0f, ladderMask);
        if (colliderLeft != null)
        {
            IsLadder = (int)LadderIs.VerticalLeft;
            ColliderLadder = colliderLeft;
            SetLadderSnapPosition(colliderLeft);
            return;
        }

        // Box droite
        Vector2 rightCenter = pos + Vector2.right * sideOffset;
        Collider2D colliderRight = Physics2D.OverlapBox(rightCenter, new Vector2(boxWidth, boxHeight), 0f, ladderMask);
        if (colliderRight != null)
        {
            IsLadder = (int)LadderIs.VerticalRight;
            ColliderLadder = colliderRight;
            SetLadderSnapPosition(colliderRight);
            return;
        }

        // Box au-dessus
        Vector2 upCenter = pos + Vector2.up * upOffset;
        Collider2D colliderUp = Physics2D.OverlapBox(upCenter, new Vector2(boxWidth, boxHeight), 0f, ladderMask);
        if (colliderUp != null)
        {
            IsLadder = (int)LadderIs.Horizontal;
            ColliderLadder = colliderUp;


            if (CurrentState is PlayerJumpState
                || CurrentState is PlayerFallState)
            {
                float distanceToLeft = Mathf.Abs(transform.position.x - colliderUp.bounds.min.x);
                float distanceToRight = Mathf.Abs(transform.position.x - colliderUp.bounds.max.x);

                if (distanceToLeft <= 0.5f)
                {
                    if (PlayerRigidbody.velocity.x < 0)
                    {
                        IsLadder = (int)LadderIs.Nothing;
                    }
                }
                else if (distanceToRight <= 0.5f)
                {
                    if (PlayerRigidbody.velocity.x > 0)
                    {
                        IsLadder = (int)LadderIs.Nothing;
                    }
                }
            }

            SetLadderSnapPosition(colliderUp);
            return;
        }
    }

    private void SetLadderSnapPosition(Collider2D collider)
    {
        CanSnapPositionLadder = true;
        float playerExtentX = PlayerCollider.bounds.extents.x + 0.1f;
        float playerExtentY = PlayerCollider.bounds.extents.y + 0.1f;

        if (IsLadder == (int)LadderIs.VerticalLeft)
        {
            float collisionRightX = collider.bounds.max.x;

            float collisionBottomY = collider.bounds.min.y;
            float collisionTopY = collider.bounds.max.y;


            float distanceToBottom = Mathf.Abs(transform.position.y - collisionBottomY);
            float distanceToTop = Mathf.Abs(transform.position.y - collisionTopY);

            if (distanceToBottom < playerExtentY)
            {
                LadderSnapPosition = new Vector2(collisionRightX + playerExtentX, collisionBottomY + playerExtentY);
            }
            else if (distanceToTop < playerExtentY)
            {
                LadderSnapPosition = new Vector2(collisionRightX + playerExtentX, collisionTopY - playerExtentY);
            }
            else
            {
                LadderSnapPosition = new Vector2(collisionRightX + playerExtentX, transform.position.y);
            }

            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, playerExtentX + 0.2f, LayerMask.GetMask("Ladder"));
            if (hit == false)
            {
                IsLadder = (int)LadderIs.Nothing;
            }
        }
        else if (IsLadder == (int)LadderIs.VerticalRight)
        {
            float collisionLeftX = collider.bounds.min.x;

            float collisionBottomY = collider.bounds.min.y;
            float collisionTopY = collider.bounds.max.y;


            float distanceToBottom = Mathf.Abs(transform.position.y - collisionBottomY);
            float distanceToTop = Mathf.Abs(transform.position.y - collisionTopY);

            if (distanceToBottom < playerExtentY)
            {
                LadderSnapPosition = new Vector2(collisionLeftX - playerExtentX, collisionBottomY + playerExtentY);
            }
            else if (distanceToTop < playerExtentY)
            {
                LadderSnapPosition = new Vector2(collisionLeftX - playerExtentX, collisionTopY - playerExtentY);
            }
            else
            {
                LadderSnapPosition = new Vector2(collisionLeftX - playerExtentX, transform.position.y);
            }
            
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, playerExtentX + 0.2f, LayerMask.GetMask("Ladder"));
            if (hit == false)
            {
                IsLadder = (int)LadderIs.Nothing;
            }
        }
        else if (IsLadder == (int)LadderIs.Horizontal)
        {
            float collisionBottomY = collider.bounds.min.y;

            float collisionLeftX = collider.bounds.min.x;
            float collisionRightX = collider.bounds.max.x;

            float distanceToLeft = Mathf.Abs(transform.position.x - collisionLeftX);
            float distanceToRight = Mathf.Abs(transform.position.x - collisionRightX);

            if (distanceToLeft < playerExtentX)
            {
                LadderSnapPosition = new Vector2(collisionLeftX + playerExtentX, collisionBottomY - playerExtentY);
            }
            else if (distanceToRight < playerExtentX)
            {
                LadderSnapPosition = new Vector2(collisionRightX - playerExtentX, collisionBottomY - playerExtentY);
            }
            else
            {
                LadderSnapPosition = new Vector2(transform.position.x, collisionBottomY - playerExtentY);
            }
        }
    }

    public void CountTimePassedInState()
    {
        TimePassedInState += Time.deltaTime;
    }

    public void ResetTimePassedInState()
    {
        TimePassedInState = 0;
    }

    public void StartGrab()
    {
        IsGrabing = true;
        StartGrabInitialisation = true;
        GrabScript.CanCountGrabBufferTime = false;
        GrabBufferCounter = 10;
        if (StartGrabInitialisation == true)
        {
            StartGrabInitialisation = false;
            GrabScript.GrabInitialisation();
        }
    }

    public void GrabBufferVerification()
    {
        if (GrabBufferCounter <= GrabBufferTime
            && Grab.ReadValue<float>() > 0)
        {
            StartGrab();
        }

    }

public void SetYPositionToGround()
    {
        //Vector2 originLeft = new Vector2(PlayerCollider.bounds.min.x, PlayerCollider.bounds.min.y);
        //Vector2 originRight = new Vector2(PlayerCollider.bounds.max.x, PlayerCollider.bounds.min.y);

        //RaycastHit2D downLeftVerification = Physics2D.Raycast(originLeft, Vector2.down, 0.1f, LayerMask.NameToLayer("Platform"));
        //RaycastHit2D downRightVerification = Physics2D.Raycast(originRight, Vector2.down, 0.1f, LayerMask.NameToLayer("Platform"));

        //if (downLeftVerification)
        //{
        //    transform.position = new Vector2(transform.position.x, downLeftVerification.collider.bounds.max.y - PlayerCollider.bounds.extents.y + 0.1f);
        //}
        //else if (downRightVerification)
        //{
        //    transform.position = new Vector2(transform.position.x, downRightVerification.collider.bounds.max.y - PlayerCollider.bounds.extents.y + 0.1f);
        //}
    }
}