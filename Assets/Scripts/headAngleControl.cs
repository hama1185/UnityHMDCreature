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

    public Text t1;
    public Text t2;
    public Text t3;
    public Text t4;
    public Text t5;


    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float headangle = ovrCam.transform.localEulerAngles.y;
        if(hangerLeftFlag){
            if(headangle >=40 && headangle <= 180){
                firstHangerFlag = true; 
                hangerLeftFlag = false;   
            }
        }
        else if(hangerRightFlag){
            if(headangle <= 320 && headangle >= 180){
                secondHangerFlag = true;
                hangerRightFlag = false;
            }
        }
        
        t1.text = hangerLeftFlag.ToString(); 
        t2.text = hangerRightFlag.ToString(); 
        t3.text = firstHangerFlag.ToString(); 
        t4.text = secondHangerFlag.ToString(); 
        t5.text = "Angle: " + headangle.ToString();
        
    }
}
