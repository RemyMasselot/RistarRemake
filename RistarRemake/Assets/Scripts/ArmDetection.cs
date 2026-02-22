using UnityEngine;

public class ArmDetection : MonoBehaviour
{
    public enum ObjectGrabedIs
    {
        Nothing = 0,
        Other = 1,
        Enemy = 2,
        LadderHorizontal = 3,
        LadderVertical = 4,
        StarHandle = 5,
        Wall = 6,
        Floor = 7,
        Ceiling= 8
    }
    public int ObjectGrabed = (int)ObjectGrabedIs.Nothing;

    [SerializeField] private PlayerStateMachine playerStateMachine;
    [SerializeField] private Transform playerTransform;

    public Vector3 SnapPosHand;
    public Vector2 SnapPosHandL;
    public Vector2 SnapPosHandR;
    [SerializeField] private Transform HandR;
    [SerializeField] private Transform HandL;

    public int rayCount = 8;         
    public float rayDistanceAdd = 0.2f;
    public float angleRange = 90f;   
    public float rotationOffset = 0f;
    public LayerMask layerMask;
    public Color gizmoColor = Color.red;
    public float SpaceBetweenRays = 0.2f;

    private void Update()
    {
        if (ObjectGrabed == (int)ObjectGrabedIs.Nothing)
        { 
            for (int i = 1; i < rayCount + 1; i++)
            {
                Vector2 perpendicular = new Vector2(-playerStateMachine.AimDir.y, playerStateMachine.AimDir.x).normalized;
                float distanceOffset = (rayCount - 1) * SpaceBetweenRays;

                Vector2 originOffset = (Vector2)playerTransform.position + perpendicular * SpaceBetweenRays * i;
                Vector2 startPointRay = originOffset - perpendicular * distanceOffset;

                Vector2 pointBetweenHands = (HandR.position + HandL.position) / 2;
                float distance = Vector2.Distance(playerTransform.position, pointBetweenHands) + rayDistanceAdd;


                RaycastHit2D hit = Physics2D.Raycast(startPointRay, playerStateMachine.AimDir, distance, layerMask);

                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                    {
                        ObjectGrabed = (int)ObjectGrabedIs.Enemy;
                        playerStateMachine.ElementGrabed = hit.collider.gameObject;
                        SetSnapPosCollider(hit);
                        break;
                    }
                    else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("StarHandle"))
                    {
                        ObjectGrabed = (int)ObjectGrabedIs.StarHandle;
                        SetSnapPosCollider(hit);
                        break;
                    }
                    else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Platform"))
                    {
                        DetecPlatform(hit);
                        break;
                    }
                    else
                    {
                        ObjectGrabed = (int)ObjectGrabedIs.Other;
                        break;
                    }
                }
            }
        }
    }

    private void DetecPlatform(RaycastHit2D hit)
    {
        LayerMask platformMask = LayerMask.GetMask("Platform");

        Vector3 verticalOffset = new Vector2(0f, 0.1f);
        Vector3 horizontalOffset = new Vector2(0.1f, 0f);

        if (playerStateMachine.AimDir.y > 0)
        {
            Vector3 startPointDetection = (Vector3)hit.point - verticalOffset;

            bool isWallRight = Physics2D.OverlapPoint(startPointDetection + horizontalOffset, platformMask) != null;
            bool isWallLeft = Physics2D.OverlapPoint(startPointDetection - horizontalOffset, platformMask) != null;

            if (isWallRight || isWallLeft)
            {
                if (IsThereALadder(hit.point) != null)
                {
                    ObjectGrabed = (int)ObjectGrabedIs.LadderVertical;
                }
                else
                {
                    ObjectGrabed = (int)ObjectGrabedIs.Wall;
                }
            } 
            else if (IsThereALadder(hit.point) != null)
            {
                ObjectGrabed = (int)ObjectGrabedIs.LadderHorizontal;
            }
            else
            {
                ObjectGrabed = (int)ObjectGrabedIs.Ceiling;
            }
        }
        else if (playerStateMachine.AimDir.y < 0)
        {
            Vector3 startPointDetection = (Vector3)hit.point + verticalOffset;

            bool isWallRight = Physics2D.OverlapPoint(startPointDetection + horizontalOffset, platformMask) != null;
            bool isWallLeft = Physics2D.OverlapPoint(startPointDetection - horizontalOffset, platformMask) != null;

            if (isWallRight || isWallLeft)
            {
                if (IsThereALadder(hit.point) != null)
                {
                    ObjectGrabed = (int)ObjectGrabedIs.LadderVertical;
                }
                else
                {
                    ObjectGrabed = (int)ObjectGrabedIs.Wall;
                }
            }
            else
            {
                ObjectGrabed = (int)ObjectGrabedIs.Floor;
            }
        }
        else
        {
            if (IsThereALadder(hit.point) != null)
            {
                ObjectGrabed = (int)ObjectGrabedIs.LadderVertical;
            }
            else
            {
                ObjectGrabed = (int)ObjectGrabedIs.Wall;
            }
        }
        SetSnapPosHitPoint(hit);
        //Debug.Log("Object Grabed : " + ObjectGrabed);
    }

    private Collider2D IsThereALadder(Vector2 worldPoint)
    {
        Collider2D hit = Physics2D.OverlapPoint(worldPoint, LayerMask.GetMask("Ladder"));
        if (hit == null) return null;
        if (!hit.isTrigger) return null;
        return hit is BoxCollider2D ? hit : null;
    }

    private void SetSnapPosHitPoint(RaycastHit2D hit)
    {
        SnapPosHand = hit.point;

        if (ObjectGrabed == (int)ObjectGrabedIs.Ceiling 
            || ObjectGrabed == (int)ObjectGrabedIs.LadderHorizontal
            || ObjectGrabed == (int)ObjectGrabedIs.Floor)
        {
            if (playerStateMachine.IsPlayerTurnToLeft)
            {
                SnapPosHandL = new Vector2(SnapPosHand.x - 0.2f, SnapPosHand.y);
                SnapPosHandR = new Vector2(SnapPosHand.x + 0.2f, SnapPosHand.y);
            }
            else
            {
                SnapPosHandL = new Vector2(SnapPosHand.x + 0.2f, SnapPosHand.y);
                SnapPosHandR = new Vector2(SnapPosHand.x - 0.2f, SnapPosHand.y);
            }
        }
        else
        {
            SnapPosHandL = new Vector2(SnapPosHand.x, SnapPosHand.y - 0.2f);
            SnapPosHandR = new Vector2(SnapPosHand.x, SnapPosHand.y + 0.2f);
        }
    }

    private void SetSnapPosCollider(RaycastHit2D hit)
    {
        SnapPosHand = hit.collider.transform.position;
        SnapPosHandL = SnapPosHand;
        SnapPosHandR = SnapPosHand;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = gizmoColor;

        //for (int i = 1; i < rayCount + 1; i++)
        //{
        //    Vector2 perpendicular = new Vector2(-playerStateMachine.AimDir.y, playerStateMachine.AimDir.x).normalized;
        //    float distanceOffset = (rayCount - 1) * SpaceBetweenRays;

        //    Vector2 originOffset = (Vector2)playerTransform.position + perpendicular * SpaceBetweenRays * i;
        //    Vector2 startPointRay = originOffset - perpendicular * distanceOffset;

        //    Vector2 pointBetweenHands = (HandR.position + HandL.position) / 2;
        //    float distance = Vector2.Distance(playerTransform.position, pointBetweenHands) + rayDistanceAdd;

        //    Gizmos.DrawRay(startPointRay, playerStateMachine.AimDir * distance);
        //}

        // Get the Point AStart which is positionned at the position of the player
        // Get the Point AEnd which is positionned at the position of the player + aim direction * distance
        // Get the perpendicular line to the segment AStart - AEnd
        // Get the Point BStart which is positionned on the perpendicular line at a distance of +0.3f from AStart
        // Get the Point BEnd which is positionned at the position of BStart + aim direction * distance
        // Do the same for CStart and CEnd but at a distance of -0.3f from AStart

        float currentDistanceGrab = Vector2.Distance(playerStateMachine.IkArmRight.position, playerStateMachine.ShoulderRight.position);

        Vector2 AStart = playerTransform.position;
        Vector2 AEnd = AStart + playerStateMachine.AimDir.normalized * currentDistanceGrab;
        Vector2 perpendicularA = new Vector2(- (AEnd - AStart).y, (AEnd - AStart).x).normalized;
        Vector2 BStart = AStart + perpendicularA * 0.3f;
        Vector2 BEnd = AEnd + perpendicularA * 0.3f;
        Vector2 CStart = AStart - perpendicularA * 0.3f;
        Vector2 CEnd = AEnd - perpendicularA * 0.3f;

        Gizmos.DrawLine(AStart, AEnd);
        Gizmos.DrawLine(BStart, BEnd);
        Gizmos.DrawLine(CStart, CEnd);
    }

    private float CalculateDistance(Vector2 origin, Vector2 direction)
    {
        //Calculer le point d'arrivée
        // Le point d'arrivée se trouve sur une droite perpendiculaire au segment entre le point d'origine
        // et le point final qui part du point d'origine dans la direction visée multipliée par la distance

        // 1 - Trouver le segment entre le point d'origine et le point final
        // Trouver le point final

        Vector2 finalPoint = origin + playerStateMachine.AimDir * 0.3f;
        Vector2 segment = finalPoint - origin;


        // 2 - Trouver le vecteur perpendiculaire à ce segment et qui passe par finalPoint

        Vector2 perpendiculaire = new Vector2(-segment.y, segment.x).normalized;


        // 3 - Trouver le point d'intersection entre la droite de la direction visée et la droite perpendiculaire au segment

        Vector2 targetPoint = GetLineIntersection(finalPoint, perpendiculaire, origin, direction);

        return Vector2.Distance(origin, targetPoint);
    }
    private static Vector2 GetLineIntersection(Vector2 p1, Vector2 dir1, Vector2 p2, Vector2 dir2)
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
