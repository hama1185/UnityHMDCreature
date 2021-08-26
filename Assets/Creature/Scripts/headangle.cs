using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class headangle : MonoBehaviour
{
    public GameObject ovrCam; //自分の頭(OVRCameraRig->trackingSpace->CenterEyeAnchor)
    public Text text; //頭部の角度表示用テキスト

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float headangle = ovrCam.transform.localEulerAngles.y;
        text.text = "Head Angle: " + headangle;
    }
}
