using UnityEngine;

public class LadderVDetectionL : MonoBehaviour
{
    [field: SerializeField] public bool IsLadderVDectectedL { get; private set; }
    
    public Vector2 CapsuleSize;
    public LayerMask LayerToCheck; // Couches à reconnaître


    void FixedUpdate()
    {
        IsLadderVDectectedL = Physics2D.OverlapCapsule(transform.position, CapsuleSize, CapsuleDirection2D.Horizontal, 0f, LayerToCheck);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, CapsuleSize);
    }
}
