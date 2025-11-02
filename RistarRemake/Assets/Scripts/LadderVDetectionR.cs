using UnityEngine;

public class LadderVDetectionR : MonoBehaviour
{
    [field: SerializeField] public bool IsLadderVDectectedR { get; private set; }

    public Vector2 CapsuleSize;
    public LayerMask LayerToCheck; // Couches à reconnaître


    void FixedUpdate()
    {
        IsLadderVDectectedR = Physics2D.OverlapCapsule(transform.position, CapsuleSize, CapsuleDirection2D.Horizontal, 0f, LayerToCheck);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, CapsuleSize);
    }
}
