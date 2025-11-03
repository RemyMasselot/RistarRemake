using UnityEngine;

public class LadderExitDetection : MonoBehaviour
{
    public enum TriggerId
    {
        TriggerUp,
        TriggerDown,
        TriggerLeft,
        TriggerRight
    }
    public TriggerId TriggerIdIs;
}