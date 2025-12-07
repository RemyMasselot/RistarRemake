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
    [FoldoutGroup("REFERENCES/Star Handle")] public GameObject TriggerGoToMeteorStrike;

    public enum LadderIs
    {
        Nothing = 0,
        VerticalLeft = 1,
        VerticalRight = 2,
        Horizontal = 3
    }
    [FoldoutGroup("REFERENCES/Detections")] public int IsLadder = (int)LadderIs.Nothing;
    [field: SerializeField, FoldoutGroup("REFERENCES/Detections")] public GroundDetection EnemyDetection { get; private set; }
    [field: SerializeField, FoldoutGroup("REFERENCES/Detections")] public GroundDetection GroundDetection { get; private set; }
    [field: SerializeField, FoldoutGroup("REFERENCES/Detections")] public LadderVDetectionL LadderVDetectionL { get; private set; }
    [field: SerializeField, FoldoutGroup("REFERENCES/Detections")] public LadderVDetectionR LadderVDetectionR { get; private set; }
    [field: SerializeField, FoldoutGroup("REFERENCES/Detections")] public LadderHDetection LadderHDetection { get; private set; }
    [field: SerializeField, FoldoutGroup("REFERENCES/Detections")] public PlatformCollisionDetection platformCollisionDetection { get; private set; }

    [FoldoutGroup("GENERAL SETTING")] public int LifesNumber = 4;
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

    [FoldoutGroup("MOVE")] public float WalkSpeed = 10;

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
    [FoldoutGroup("GRAB")] public float DistanceGrab = 0.8f;
    [FoldoutGroup("GRAB")] public float TimeToExtendArms = 0.2f;
    [FoldoutGroup("GRAB")] public float MaxHoldGrabTime = 1;
    [FoldoutGroup("GRAB")] public float MaxTimeStayingAtApex = 0.3f;
    [FoldoutGroup("GRAB")] public float FallInGrabValue = -6;
    [FoldoutGroup("GRAB")] public float HorizontalGrabMovementMultiplier = 1.5f;
    [FoldoutGroup("GRAB")] public float VerticalGrabMovementMultiplier = 1;

    [FoldoutGroup("HEADBUTT")] public float HeadbuttMoveSpead;
    [HideInInspector] public Vector2 HeadbuttDirection;

    [FoldoutGroup("SPIN")] public float SpinTime;

    [HideInInspector] public Vector3 StarHandleCentre;
    [HideInInspector] public float StarHandleCurrentValue = 0;
    [FoldoutGroup("STAR HANDLE")] public float StarHandleTargetValue = 200;
    [FoldoutGroup("STAR HANDLE")] public float StarHandleRayonMin = 1.5f;
    [FoldoutGroup("STAR HANDLE")] public float StarHandleRayonMax = 2.5f;
    [HideInInspector] public float StarHandleCurrentRayon = 1.5f;
    [FoldoutGroup("STAR HANDLE")] public float StarHandleMinSpeed = 6;
    [FoldoutGroup("STAR HANDLE")] public float StarHandleMaxSpeed = 12;
    [HideInInspector] public float StarHandleCurrentSpeed = 6;
    [FoldoutGroup("STAR HANDLE")] public float StarHandleSpeedSlowMotion = 8;
    [FoldoutGroup("STAR HANDLE")] public float StarHandleImpulseMin = 10;
    [FoldoutGroup("STAR HANDLE")] public float StarHandleImpulseMax = 30;
    [HideInInspector] public float StarHandleCurrentImpulse = 0;
    
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
        if (PlayerRigidbody.velocity.x != 0) // VELOCITY
        {
            IsPlayerTurnToLeft = PlayerRigidbody.velocity.x < 0;
        }
        else if (IsLadder == (int)LadderIs.VerticalLeft || IsLadder == (int)LadderIs.VerticalRight) // VERTICAL LADDER
        {
            if (LadderVDetectionL.IsLadderVDectectedL == true)
            {
                IsPlayerTurnToLeft = true;
            }
            if (LadderVDetectionR.IsLadderVDectectedR == true)
            {
                IsPlayerTurnToLeft = false;
            }
        }
    }

    public void LadderVerif(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("LadderV"))
        {
            if (LadderVDetectionL.IsLadderVDectectedL == true)
            {
                IsLadder = (int)LadderIs.VerticalLeft;
            }
            else if (LadderVDetectionR.IsLadderVDectectedR == true)
            {
                IsLadder = (int)LadderIs.VerticalRight;
            }
        }
        else if (collision.gameObject.CompareTag("LadderH"))
        {
            IsLadder = (int)LadderIs.Horizontal;
        }
        else
        {
            IsLadder = (int)LadderIs.Nothing;
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
        if (StartGrabInitialisation == true)
        {
            StartGrabInitialisation = false;
            GrabScript.GrabInitialisation();
        }
    }
}