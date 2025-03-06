using UnityEngine;

public class GroundDetection : MonoBehaviour
{
    public bool IsLayerDectected { get; private set; }
    
    public Vector2 CapsuleSize;
    public LayerMask LayerToCheck; // Couches à reconnaître


    void FixedUpdate()
    {
        IsLayerDectected = Physics2D.OverlapCapsule(transform.position, CapsuleSize, CapsuleDirection2D.Horizontal, 0f, LayerToCheck);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, CapsuleSize);
    }
}
