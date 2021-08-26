using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class viewHacking : MonoBehaviour
{
    OVRPassthroughLayer passthroughLayer;
    private float alpha = 0.0f;
    private float brightness = 0.0f;
    public Text text;
    public Text edgerenderText;
    bool edgeRendering = false;
    bool changing = false;
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
        if(edgeRendering){
            passthroughLayer.edgeRenderingEnabled = true;
            edgerenderText.text = "EdgeRender: ON";
        } else {
            passthroughLayer.edgeRenderingEnabled = false;
            edgerenderText.text = "EdgeRender: OFF";
        }
        time += Time.deltaTime;
        float T = 60.0f;
        float f = 1.0f / T;
        if(changing){
            if(time < 30.0f){
                Color a = Color.black;
                //Color a = Color.Lerp(Color.black, Color.green, time/30.0f);
                Color b = Color.Lerp(Color.white, Color.blue, time/30.0f);
                passthroughLayer.SetColorMapControls(0.7f, 0.3f, 0.0f, makeGradient(a,b));
                //Color a = Color.Lerp(Color.black, Color.black, time/60.0f);
                //Color b = Color.Lerp(Color.white, Color.white, time/60.0f);

                //passthroughLayer.SetColorMapControls(1.0f, 0.0f, 0.0f);
                //passthroughLayer.edgeRenderingEnabled = true;
                //Color col = new Color(0, 1, 0, 1);
                //passthroughLayer.edgeColor = col;
                //alpha = Mathf.Abs(Mathf.Sin(2 * Mathf.PI * f * Time.time) * 0.3f);
            } else{ 
                Color a = Color.black;
                //Color a = Color.Lerp(Color.black, Color.green, 1.0f);
                Color b = Color.Lerp(Color.white, Color.blue, 1.0f);

                //passthroughLayer.SetColorMapControls(1.0f, 0.0f, 0.0f);
                //passthroughLayer.edgeRenderingEnabled = true;
                //Color col = new Color(0, 1, 0, 1);
                //passthroughLayer.edgeColor = col;
                passthroughLayer.SetColorMapControls(0.7f, 0.3f, 0.0f, makeGradient(a,b));
            }
        }
        text.text = "Time: " + time;
        //passthroughLayer.SetColorMap(col.r, col.g, col.b, col.a);
        //passthroughLayer.colorMapEditorContrast = 1.0f;
        //passthroughLayer.colorMapEditorBrightness = 0.0f;
        //passthroughLayer.colorMapEditorPosterize = 0.0f;
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
}
