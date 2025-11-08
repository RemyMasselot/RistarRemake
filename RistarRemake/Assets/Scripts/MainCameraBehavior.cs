using Sirenix.OdinInspector;
using UnityEngine;

public class MainCameraBehavior : MonoBehaviour
{
    #region VARIABLES

    [SerializeField, FoldoutGroup("RÉFÉRENCES")] private PlayerStateMachine playerStateMachine;
    [FoldoutGroup("RÉFÉRENCES")] private Transform playerTransform;

    private Vector3 targetPosition;
    private Vector3 currentPosition;

    private float lerpSpeed;
    [SerializeField, FoldoutGroup("MOVEMENT")] private float lerpSpeedGlobal = 5f;
    [SerializeField, FoldoutGroup("MOVEMENT")] private float lerpSpeedMeteorStrike = 15f;
    
    #endregion

    private void Start()
    {
        playerTransform = playerStateMachine.gameObject.transform;
        targetPosition = playerTransform.position;
        currentPosition = targetPosition;
        
        lerpSpeed = lerpSpeedGlobal;
    }

    private void LateUpdate()
    {
        if (playerStateMachine.CurrentState is PlayerHangState
            || playerStateMachine.CurrentState is PlayerHeadbuttState)
        {
            targetPosition = playerStateMachine.CameraTargetOverride;
        }
        else
        {
            targetPosition = playerTransform.position;
        }

        if (playerStateMachine.CurrentState is PlayerMeteorStrikeState)
        {
            targetPosition = playerTransform.position + (Vector3)playerStateMachine.PlayerRigidbody.velocity.normalized * 2f;
            lerpSpeed = lerpSpeedMeteorStrike;
        }
        else
        {
            lerpSpeed = lerpSpeedGlobal;
        }

        currentPosition = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime * lerpSpeed);
        currentPosition.z = -10f;
        transform.position = currentPosition;
    }
}
