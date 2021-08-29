using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class headAngleControl : MonoBehaviour
{
    public GameObject ovrCam; //自分の頭(OVRCameraRig->trackingSpace->CenterEyeAnchor)
    public bool hangerLeftFlag = false; // 最初
    public bool hangerRightFlag = false; // 最後

    public bool firstHangerFlag = false;
    public bool secondHangerFlag = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float headangle = ovrCam.transform.localEulerAngles.y;
        if(hangerLeftFlag){
            if(headangle >=5 && headangle <= 180){
                firstHangerFlag = true;    
            }
        }
        else if(hangerRightFlag){
            if(headangle <= 355 && headangle >= 180){
                secondHangerFlag = true;
            }
        }
        
    }
}
