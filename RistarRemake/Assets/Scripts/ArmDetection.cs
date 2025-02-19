using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmDetection : MonoBehaviour
{
    //public ArmDetection(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) 
    //: base(currentContext, playerStateFactory) { }

    public int ObjectDetected = 0;
    public PlayerGrabState PlayerGrabState;
    public bool EndAnim = false;


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ladder"))
        {
            ObjectDetected = 3;
            PlayerGrabState.GrabDetectionVerif(ObjectDetected);
        }
        else if(collision.gameObject.CompareTag("Enemy"))
        {
            ObjectDetected = 2;
            PlayerGrabState.GrabDetectionVerif(ObjectDetected);
        }
        else
        {
            ObjectDetected = 1;
            PlayerGrabState.GrabDetectionVerif(ObjectDetected);
        }
    }
}
