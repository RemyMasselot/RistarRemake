using UnityEngine;

public class LadderExitDetection : MonoBehaviour
{
    [SerializeField] private PlayerStateMachine playerStateMachine;
    public bool IsLayerDectected { get; private set; }

    public Vector2 CapsuleSize;
    public LayerMask LayerToCheck; // Couches à reconnaître

    private enum TriggerState
    {
        TriggerUp,
        TriggerDown,
        TriggerLeft,
        TriggerRight
    }
    [SerializeField] private TriggerState triggerState;


    void FixedUpdate()
    {
        IsLayerDectected = Physics2D.OverlapCapsule(transform.position, CapsuleSize, CapsuleDirection2D.Horizontal, 0f, LayerToCheck);

        if (IsLayerDectected == true)
        {
            //Debug.Log(IsLayerDectected);
            //Debug.Log("Value 1 : " + playerStateMachine.CurrentState.GetType());
            //Debug.Log("Value 2 : " + playerStateMachine._states.WallClimb().GetType());
            //Debug.Log("Resultat : " + (playerStateMachine.CurrentState.GetType() == playerStateMachine._states.WallClimb().GetType()));
            if (playerStateMachine.CurrentState.GetType() == playerStateMachine._states.WallClimb().GetType())
            {
                Debug.Log("WALL CLIMB");
                if (playerStateMachine.gameObject.GetComponent<Rigidbody2D>().velocity.y > 0f)
                {
                    if (triggerState == TriggerState.TriggerUp)
                    {
                        playerStateMachine.Leap = true;
                        Debug.Log("Leap");
                        //Leap();
                    }
                }
                if (playerStateMachine.gameObject.GetComponent<Rigidbody2D>().velocity.y < 0f)
                {
                    if (triggerState == TriggerState.TriggerDown)
                    {
                        playerStateMachine.Fall = true;
                        Debug.Log("Fall");
                        //Fall();
                    }
                }
                if (playerStateMachine.gameObject.GetComponent<Rigidbody2D>().velocity.x > 0f)
                {
                    if (triggerState == TriggerState.TriggerRight)
                    {
                        playerStateMachine.Fall = true;
                        Debug.Log("Fall");
                        //Leap();
                    }
                }
                if (playerStateMachine.gameObject.GetComponent<Rigidbody2D>().velocity.x < 0f)
                {
                    if (triggerState == TriggerState.TriggerLeft)
                    {
                        playerStateMachine.Fall = true;
                        Debug.Log("Fall");
                        //Fall();
                    }
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, CapsuleSize);
    }

    private void Leap()
    {
        Debug.Log("Leap");
    }

    private void Fall()
    {
        Debug.Log("Fall");
    }
}
