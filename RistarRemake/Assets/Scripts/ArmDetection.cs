using UnityEngine;

public class ArmDetection : MonoBehaviour
{
    public enum ObjectDetectedIs
    {
        Nothing = 0,
        Other = 1,
        Enemy = 2,
        Ladder = 3,
        StarHandle = 4,
        Wall = 5,
        Floor = 6
    }
    public int ObjectDetected = (int)ObjectDetectedIs.Nothing;

    public Vector2 SnapPosHand;
    public Vector2 SnapPosHandL;
    public Vector2 SnapPosHandR;
    [SerializeField] private Transform HandR;
    [SerializeField] private Transform HandL;

    public int rayCount = 8;         
    public float rayDistance = 5f;   
    public float angleRange = 90f;   
    public float rotationOffset = 0f;
    public LayerMask layerMask;
    public Color gizmoColor = Color.red;

    private void Update()
    {
        Vector3 origin = transform.position;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = rotationOffset - (angleRange / 2f) + (i * angleRange / (rayCount - 1));
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            RaycastHit2D hit = Physics2D.Raycast(origin, direction, rayDistance, layerMask);

            if (hit.collider != null)
            {
                if (hit.collider.CompareTag("Floor"))
                {
                    SnapPosHand = hit.collider.gameObject.transform.position;
                    ObjectDetected = (int)ObjectDetectedIs.Floor;
                }
                else if (hit.collider.CompareTag("Wall"))
                {
                    SnapPosHandL = HandL.position;
                    SnapPosHandR = HandR.position;
                    SnapPosHand = (HandL.position + HandR.position) / 2;
                    ObjectDetected = (int)ObjectDetectedIs.Wall;
                }
                else if (hit.collider.CompareTag("StarHandle"))
                {
                    SnapPosHand = hit.collider.gameObject.transform.position;
                    ObjectDetected = (int)ObjectDetectedIs.StarHandle;
                }
                else if (hit.collider.CompareTag("LadderV") || hit.collider.CompareTag("LadderH"))
                {
                    SnapPosHandL = HandL.position;
                    SnapPosHandR = HandR.position;
                    ObjectDetected = (int)ObjectDetectedIs.Ladder;
                }
                else if (hit.collider.CompareTag("Enemy"))
                {
                    SnapPosHand = hit.collider.gameObject.transform.position;
                    ObjectDetected = (int)ObjectDetectedIs.Enemy;
                }
                else
                {
                    ObjectDetected = (int)ObjectDetectedIs.Other;
                }
            }
            //else
            //{
            //    ObjectDetected = (int)ObjectDetectedIs.Nothing;
            //}
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;
        Vector3 origin = transform.position;

        for (int i = 0; i < rayCount; i++)
        {
            // Calcul de l'angle pour chaque rayon
            float angle = rotationOffset - (angleRange / 2f) + (i * angleRange / (rayCount - 1));
            Vector3 direction = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
            Gizmos.DrawRay(origin, direction * rayDistance);
        }
    }
}
