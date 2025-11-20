using Sirenix.OdinInspector;
using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    #region VARIABLES

    private PlayerStateMachine playerStateMachine;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    [SerializeField, FoldoutGroup("RÉFÉRENCES")] private SpriteRenderer handRight;
    [SerializeField, FoldoutGroup("RÉFÉRENCES")] private SpriteRenderer handLeft;
    [SerializeField, FoldoutGroup("RÉFÉRENCES")] private Sprite handOpen;
    [SerializeField, FoldoutGroup("RÉFÉRENCES")] private Sprite handClose;
    [SerializeField, FoldoutGroup("RÉFÉRENCES")] private GameObject arms;
    [SerializeField, FoldoutGroup("RÉFÉRENCES")] private LineRenderer lineArmRight;
    [SerializeField, FoldoutGroup("RÉFÉRENCES")] private LineRenderer lineArmLeft;

    #endregion

    private void Awake()
    {
        playerStateMachine = GetComponentInParent<PlayerStateMachine>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerStateMachine.NewStatePlayed.AddListener(OnNewStatePlayed);
        playerStateMachine.StartGrab.AddListener(OnNewStatePlayed);
    }

    private void Update()
    {
        ChangePlayerDirection();

        ArmVisibility();

        if (playerStateMachine.CurrentState is PlayerGrabState || playerStateMachine.IsGrabing) // GRAB STATE ONGOING
        {
            if (playerStateMachine.ArmDetection.ObjectDetected != 0)
            {
                // HANDS CLOSE
                handRight.sprite = handClose;
                handLeft.sprite = handClose;
            }
        }
        else if (playerStateMachine.CurrentState is PlayerMeteorStrikeState) // METEOR STRIKE ONGOING
        {
            MeteorStrikeBodyRotation();
        }
    }

    private void ChangePlayerDirection()
    {
        spriteRenderer.flipX = playerStateMachine.IsPlayerTurnToLeft ? true : false;
    }

    private void OnNewStatePlayed()
    {
        // NEW ANIMATION
        ChooseAnimationOnEnterNewState();

        // PUT BACK THE BODY ROTATION TO 0 IF NOT IN METEOR STRIKE
        if (playerStateMachine.CurrentState is not PlayerMeteorStrikeState)
        {
            spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, 0);
            spriteRenderer.flipY = false;
        }
    }

    private void ChooseAnimationOnEnterNewState()
    {
        if (playerStateMachine.CurrentState is PlayerIdleState && playerStateMachine.IsGrabing == false)
        {
            animator.SetTrigger("Idle");
        }
        else if (playerStateMachine.CurrentState is PlayerWalkState)
        {
            animator.SetTrigger("Walk");
        }
        else if (playerStateMachine.CurrentState is PlayerJumpState)
        {
            animator.SetTrigger("Jump");
        }
        else if (playerStateMachine.CurrentState is PlayerFallState)
        {
            animator.SetTrigger("Fall");
        }
        else if (playerStateMachine.CurrentState is PlayerGrabState || playerStateMachine.IsGrabing)
        {
            EnterGrabStateInitialization();
            ChoiceGrabAnimation();
            animator.SetTrigger("Grab");
        }
        else if (playerStateMachine.CurrentState is PlayerHangState)
        {
            animator.SetFloat("HangValue", playerStateMachine.ArmDetection.ObjectDetected == 4 ? 2 : 1);
            animator.SetTrigger("Hang");
        }
        else if (playerStateMachine.CurrentState is PlayerMeteorStrikeState)
        {
            animator.SetTrigger("MeteorStrike");
        }
        else if (playerStateMachine.CurrentState is PlayerHeadbuttState)
        {
            animator.SetTrigger("Headbutt");
        }
        else if (playerStateMachine.CurrentState is PlayerSpinState)
        {
            animator.SetTrigger("Spin");
        }
        else if (playerStateMachine.CurrentState is PlayerWallIdleState)
        {
            ChooseBetweenVerticalOrHorizontalAnimationLadder();
            animator.SetTrigger("WallIdle");
        }
        else if (playerStateMachine.CurrentState is PlayerWallClimbState)
        {
            ChooseBetweenVerticalOrHorizontalAnimationLadder();
            animator.SetTrigger("WallClimb");
        }
        else if (playerStateMachine.CurrentState is PlayerWallJumpState)
        {
            animator.SetTrigger("Jump");
        }
        else if (playerStateMachine.CurrentState is PlayerLeapState)
        {
            animator.SetTrigger("Jump");
        }
        else if (playerStateMachine.CurrentState is PlayerDamageState)
        {
            animator.SetTrigger("Damage");
        }
        else if (playerStateMachine.CurrentState is PlayerDeathState)
        {
            if (playerStateMachine.IsPlayerTurnToLeft)
            {
                animator.SetTrigger("DeathR");
            }
            else
            {
                animator.SetTrigger("DeathL");
            }
        }
    }

    private void ChooseBetweenVerticalOrHorizontalAnimationLadder()
    {
        if (playerStateMachine.IsLadder == (int)PlayerStateMachine.LadderIs.VerticalLeft || playerStateMachine.IsLadder == (int)PlayerStateMachine.LadderIs.VerticalRight)
        {
            animator.SetFloat("WallVH", 0);
        }
        else if (playerStateMachine.IsLadder == (int)PlayerStateMachine.LadderIs.Horizontal)
        {
            animator.SetFloat("WallVH", 1);
        }
    }

    private void EnterGrabStateInitialization()
    {
        handRight.sprite = handOpen;
        handLeft.sprite = handOpen;

        if (playerStateMachine.IsPlayerTurnToLeft == false)
        {
            lineArmLeft.sortingOrder = 0;
            lineArmRight.sortingOrder = 10;
        }
        else
        {
            lineArmLeft.sortingOrder = 10;
            lineArmRight.sortingOrder = 0;
        }
    }

    private void ChoiceGrabAnimation()
    {
        // Vérification d'un sol ou non
        if (playerStateMachine.GroundDetection.IsGroundDectected)
        {
            if (playerStateMachine.AimDir.y <= 0.6f)
            {
                animator.SetFloat("GrabAnimId", 1);
            }
            else
            {
                animator.SetFloat("GrabAnimId", 2);
            }
        }
        else
        {
            if (playerStateMachine.AimDir.y <= -0.2f)
            {
                animator.SetFloat("GrabAnimId", 5);
            }
            else if (playerStateMachine.AimDir.y >= 0.2f)
            {
                animator.SetFloat("GrabAnimId", 4);
            }
            else
            {
                animator.SetFloat("GrabAnimId", 3);
            }
        }
    }

    private void ArmVisibility()
    {
        // Draw Line Arm
        lineArmLeft.SetPosition(0, playerStateMachine.ShoulderLeft.position);
        lineArmLeft.SetPosition(1, playerStateMachine.IkArmLeft.position);
        lineArmRight.SetPosition(0, playerStateMachine.ShoulderRight.position);
        lineArmRight.SetPosition(1, playerStateMachine.IkArmRight.position);
        
        if (playerStateMachine.CurrentState is PlayerGrabState || playerStateMachine.IsGrabing)
        {
            HandsRotation();
            arms.SetActive(true);
        }
        else if (playerStateMachine.CurrentState is PlayerHangState)
        {
            HandsRotation();
            arms.SetActive(true);
        }
        else if (playerStateMachine.CurrentState is PlayerHeadbuttState)
        {
            HandsRotation();
            arms.SetActive(true);
        }
        else
        {
            arms.SetActive(false);
        }
    }

    private void HandsRotation()
    {
        float angle = Mathf.Atan2(playerStateMachine.AimDir.x, playerStateMachine.AimDir.y) * Mathf.Rad2Deg;
        Quaternion _dirQ = Quaternion.Euler(new Vector3(0, 0, -angle + 90));

        playerStateMachine.IkArmRight.transform.rotation = _dirQ;
        playerStateMachine.IkArmLeft.transform.rotation = _dirQ;
    }

    private void MeteorStrikeBodyRotation()
    {
        float angle = Mathf.Atan2(playerStateMachine.MeteorStrikeDirection.y, playerStateMachine.MeteorStrikeDirection.x) * Mathf.Rad2Deg;
        spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, angle);

        if (playerStateMachine.MeteorStrikeDirection.x > 0)
        {
            spriteRenderer.flipY = false;
        }
        if (playerStateMachine.MeteorStrikeDirection.x < 0)
        {
            spriteRenderer.flipY = true;
        }
    }
}
