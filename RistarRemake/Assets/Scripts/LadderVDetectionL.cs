using UnityEngine;

public class LadderVDetectionL : MonoBehaviour
{
    [field: SerializeField] public int IsLadderVDectectedL { get; private set; }
    
    public Vector2 CapsuleSize;
    public LayerMask LayerToCheck; // Couches � reconna�tre


    void FixedUpdate()
    {
        Collider2D hit = Physics2D.OverlapCapsule(transform.position, CapsuleSize, CapsuleDirection2D.Horizontal, 0f, LayerToCheck);
        if (hit != null)
        {
            IsLadderVDectectedL = hit.gameObject.layer;
            //if (hit.gameObject.layer == LayerMask.NameToLayer("LadderV"))
            //{
            //    IsLadderVDectectedL = 1;
            //}
            //if (hit.gameObject.layer == LayerMask.NameToLayer("Wall"))
            //{
            //    IsLadderVDectectedL = 2;
            //}
            //else
            //{
            //    IsLadderVDectectedL = 0;
            //}
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, CapsuleSize);
    }
}
