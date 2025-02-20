using UnityEngine;

public class LayerDetection : MonoBehaviour
{
    public Vector2 CapsuleSize;
    public LayerMask LayerToCheck; // Couches à reconnaître

    public bool IsLayerDectected { get; private set; }

    void Update()
    {
        IsLayerDectected = Physics2D.OverlapCapsule(transform.position, CapsuleSize, CapsuleDirection2D.Horizontal, 0f, LayerToCheck);
        //Debug.Log(IsGrounded);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, CapsuleSize);
    }
}
