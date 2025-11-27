using Unity.VisualScripting;
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

    [SerializeField] private PlayerStateMachine playerStateMachine;

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
        Vector2 origin = transform.position;

        for (int i = 0; i < rayCount; i++)
        {
            float angle = rotationOffset - (angleRange / 2f) + (i * angleRange / (rayCount - 1));
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
            float distance = rayDistance;

            RaycastHit2D hit = Physics2D.Raycast(origin, direction, distance, layerMask);

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
        Vector2 origin = transform.position;

        for (int i = 0; i < rayCount; i++)
        {
            // Calcul de l'angle pour chaque rayon
            float angle = rotationOffset - (angleRange / 2f) + (i * angleRange / (rayCount - 1));
            Vector2 direction = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad)).normalized;


            //Calculer le point d'arrivée
            // Le point d'arrivée se trouve sur une droite perpendiculaire au segment entre le point d'origine
            // et le point final qui part du point d'origine dans la direction visée multipliée par la distance

            // 1 - Trouver le segment entre le point d'origine et le point final
            // Trouver le point final

            Vector2 finalPoint = origin + playerStateMachine.AimDir * rayDistance;
            Vector2 segment = finalPoint - origin;


            // 2 - Trouver le vecteur perpendiculaire à ce segment et qui passe par finalPoint

            Vector2 perpendiculaire = new Vector2(-segment.y, segment.x).normalized;


            // 3 - Trouver le point d'intersection entre la droite de la direction visée et la droite perpendiculaire au segment

            Vector2 targetPoint = GetLineIntersection(finalPoint, perpendiculaire, origin, direction);
            
            float distance = Vector2.Distance(origin, targetPoint);


            Gizmos.DrawRay(origin, direction * distance); 
            //Debug.Log(targetPoint);
        }
    }

    public static Vector2 GetLineIntersection(Vector2 p1, Vector2 dir1, Vector2 p2, Vector2 dir2)
    {
        float a1 = -dir1.y;
        float b1 = dir1.x;
        float c1 = a1 * p1.x + b1 * p1.y;

        float a2 = -dir2.y;
        float b2 = dir2.x;
        float c2 = a2 * p2.x + b2 * p2.y;

        float det = a1 * b2 - a2 * b1;

        // Calcul du point d'intersection
        float x = (b2 * c1 - b1 * c2) / det;
        float y = (a1 * c2 - a2 * c1) / det;

        return new Vector2(x, y);
    }
}
