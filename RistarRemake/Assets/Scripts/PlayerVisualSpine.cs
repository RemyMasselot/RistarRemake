using Spine.Unity;
using UnityEngine;

public class PlayerVisualSpine : MonoBehaviour
{
    private PlayerStateMachine playerStateMachine;
    private SkeletonAnimation skeletonAnimation;

    private void Awake()
    {
        playerStateMachine = GetComponentInParent<PlayerStateMachine>();
        skeletonAnimation = GetComponent<SkeletonAnimation>();
    }

    private void Update()
    {
        // PLAYER DIRECTION
        skeletonAnimation.skeleton.ScaleX = playerStateMachine.IsPlayerTurnToLeft ? -1 : 1;
    }
}
