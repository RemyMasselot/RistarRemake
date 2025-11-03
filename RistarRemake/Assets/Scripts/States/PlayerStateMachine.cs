using Sirenix.OdinInspector;
using Spine.Unity;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    ///////////////////////////////////////////////////////       VARIABLES       ///////////////////////////////////////////////////////
    // STATES
    public PlayerStateFactory _states;
    private PlayerBaseState _currentState;
    public PlayerBaseState CurrentState { get { return _currentState; } set { _currentState = value; } }
    public PlayerBaseState PreviousState;
    [HideInInspector] public bool IsNewState;

    [SerializeField, FoldoutGroup("INPUT ACTIONS")] private Controller controls;
    [SerializeField, FoldoutGroup("INPUT ACTIONS")] public InputAction MoveH;
    [SerializeField, FoldoutGroup("INPUT ACTIONS")] public InputAction MoveV;
    [SerializeField, FoldoutGroup("INPUT ACTIONS")] public InputAction Jump;
    [SerializeField, FoldoutGroup("INPUT ACTIONS")] public InputAction Grab;
    [SerializeField, FoldoutGroup("INPUT ACTIONS")] public InputAction Aim;
    [SerializeField, FoldoutGroup("INPUT ACTIONS")] public InputAction Back;

    // CAMERA
    public Camera Camera;
    public MainCameraBehavior MainCameraBehavior;
    [HideInInspector] public bool CameraImpacted;
    [HideInInspector] public bool CameraInde;
    [HideInInspector] public Vector3 CameraTargetOverride;
    
    // ANIM
    public bool IsPlayerTurnToLeft = false;
    public bool UseSpine = false;
    public SkeletonAnimation SkeletonAnimation;
    public Animator Animator;

    // General Setting
    public int LifeNumber = 4;


    // MOVE
    public float WalkSpeed = 10;

    [FoldoutGroup("JUMP")] public float JumpForceV = 6f;
    [FoldoutGroup("JUMP")] public float JumpForceH = 4f;
    [FoldoutGroup("JUMP")] public float MaxTimeJump;
    [FoldoutGroup("JUMP")] public float CurrentTimerValueJump;
    [FoldoutGroup("JUMP")] public bool IsTimerRunningJump = false;
    [FoldoutGroup("JUMP")] public CornerCorrection CornerCorrection;
    [FoldoutGroup("JUMP")] public float JumpBufferTime = 0.1f;
    [FoldoutGroup("JUMP")] public float CoyoteTime = 0.1f;
    [FoldoutGroup("JUMP")] public float JumpBufferCounter;
    [FoldoutGroup("JUMP")] public float CoyoteCounter;
    [FoldoutGroup("JUMP")] public bool JumpReady = false;

    [FoldoutGroup("FALL")] public float MoveDownFallValue = -0.2f;
    [FoldoutGroup("FALL")] public float MoveDownFallValueMax = 1f;

    [FoldoutGroup("LEAP")] public float LeapForceV = 7f;
    [FoldoutGroup("LEAP")] public float LeapForceH = 3f;

    [field: HideInInspector] public bool Leap { get; set; } = false;
    [field: HideInInspector] public bool Fall { get; set; } = false;


    // GRAB
    public bool GamepadUsed;
    public GameObject Arms;
    [field: SerializeField] public ArmDetection ArmDetection { get; set; }
    [HideInInspector] public Vector2 AimDir;
    public float DistanceGrab;
    public float DurationExtendGrab;
    public float MaxTimeGrab;
    public float CurrentTimerValue;
    public bool IsTimerRunning = false;
    public Transform IkArmRight;
    public LineRenderer LineArmRight;
    public Transform ShoulderRight;
    public Transform DefaultPosRight;
    public Transform IkArmLeft;
    public LineRenderer LineArmLeft;
    public Transform ShoulderLeft;
    public Transform DefaultPosLeft;


    // STAR HANDLE
    public float _starHandleCurrentValue = 0;
    public float StarHandleTargetValue = 200;
    public Vector3 ShCentre;
    public float ShRayonMin = 1.5f;
    public float ShRayon = 1.5f;
    public float ShRayonMax = 2.5f;
    public float ShMinSpeed = 6;
    public float ShSpeed = 6;
    public float ShMaxSpeed = 12;
    public float ShImpulseCurrent = 0;
    public float ShImpulseMin = 10;
    public float ShImpulseMax = 30;
    public float ShSpeedSlowMotion = 8;
    public GameObject TriggerGoToMeteorStrike;

    // METEOR STRIKE
    public float MeteorSpeed = 10;
    public bool IsTimerRunningMeteor = true;
    public float MaxTimeMeteor = 10;
    public float CurrentTimerValueMeteor = 0;
    public Vector2 MeteorStrikeDirection;

    // PHYSICS
    public Rigidbody2D PlayerRigidbody { get { return GetComponent<Rigidbody2D>(); } }
    public Invincinbility Invincinbility;
    
    [field: SerializeField] public GroundDetection EnemyDetection { get; private set; }
    [field: SerializeField] public GroundDetection GroundDetection { get; private set; }
    [field: SerializeField] public GroundDetection JumpBufferingDetection { get; private set; }
    [field: SerializeField] public LadderVDetectionL LadderVDetectionL { get; private set; }
    [field: SerializeField] public LadderVDetectionR LadderVDetectionR { get; private set; }
    [field: SerializeField] public LadderHDetection LadderHDetection { get; private set; }
    [field: SerializeField] public LadderExitDetection LadderExitDetection { get; private set; }
    public enum LadderIs
    {
        Nothing = 0,
        VerticalLeft = 1,
        VerticalRight = 2,
        Horizontal = 3
    }
    public int IsLadder = (int)LadderIs.Nothing;


    private void Awake()
    {
        // setup state
        _states = new PlayerStateFactory(this);
        _currentState = _states.Idle();
        _currentState.EnterState();
    }

    ///////////////////////////////////////////////////////       START       ///////////////////////////////////////////////////////
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
        _currentState.UpdateState();
    }

    void FixedUpdate()
    {
        _currentState.FixedUpdateState();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        CurrentState.OnCollisionEnter2D(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        CurrentState.OnCollisionStay2D(collision);
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

    //public void UpdateAnim(string animName)
    //{
    //    if (UseSpine == false)
    //    {
    //        foreach (var param in Animator.parameters)
    //        {
    //            if (param.type == AnimatorControllerParameterType.Bool)
    //            {
    //                Animator.SetBool(param.name, false);
    //            }
    //        }
    //        Animator.SetBool(animName, true);
    //    }
    //}

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

    public void NewStateVerif()
    {
        IsNewState = CurrentState != PreviousState;
    }
}