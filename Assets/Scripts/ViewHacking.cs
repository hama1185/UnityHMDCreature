using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ViewHacking : MonoBehaviour
{
    OVRPassthroughLayer passthroughLayer;
    bool changing = true;
    float time = 0.0f;
    void Start()
    {
        GameObject ovrCameraRig = GameObject.Find("OVRCameraRig");
        if (ovrCameraRig == null)
        {
            Debug.LogError("Scene does not contain an OVRCameraRig");
            return;
        }
        passthroughLayer = ovrCameraRig.GetComponent<OVRPassthroughLayer>();
        if (passthroughLayer == null)
        {
            Debug.LogError("OVRCameraRig does not contain an OVRPassthroughLayer component");
        }
        passthroughLayer.SetColorMapControls(0.7f, 0.0f, 0.0f, makeGradient(Color.black,Color.white));
    }
    void Update()
    {
    }
    public void OnButtonClick(){
        //edgeRendering = !edgeRendering;
        time = 0.0f;
        changing = true;
    }
    private Gradient makeGradient(Color a, Color b){
        Gradient gradient = new Gradient();
        GradientColorKey[] colorKey;
        GradientAlphaKey[] alphaKey;
        // Populate the color keys at the relative time 0 and 1 (0 and 100%)
        colorKey = new GradientColorKey[2];
        colorKey[0].color = a;
        colorKey[0].time = 0.0f;
        colorKey[1].color = b;
        colorKey[1].time = 1.0f;
        // Populate the alpha  keys at relative time 0 and 1  (0 and 100%)
        alphaKey = new GradientAlphaKey[2];
        alphaKey[0].alpha = 1.0f;
        alphaKey[0].time = 0.0f;
        alphaKey[1].alpha = 0.0f;
        alphaKey[1].time = 1.0f;
        gradient.SetKeys(colorKey, alphaKey);
        return gradient;
    }
    public void viewHack(float t){
        if(t < 90.0f){
            Color a = Color.black;
            Color b = Color.Lerp(Color.white, Color.blue, t/90.0f);
            passthroughLayer.SetColorMapControls(0.7f, 0.3f, 0.0f, makeGradient(a,b));
        } else {
            Color a = Color.black;
            Color b = Color.Lerp(Color.blue, Color.white, (t-90.0f)/90.0f);
            passthroughLayer.SetColorMapControls(0.7f, 0.3f, 0.0f, makeGradient(a,b));
        }
    }
}