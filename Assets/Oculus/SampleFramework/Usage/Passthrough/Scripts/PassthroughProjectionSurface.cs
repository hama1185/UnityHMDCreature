using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassthroughProjectionSurface : MonoBehaviour
{
    private OVRPassthroughLayer passthroughLayer;
    public MeshFilter projectionObject;
    MeshRenderer quadOutline;
    float add = 0.1f;

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

        passthroughLayer.AddSurfaceGeometry(projectionObject.gameObject, true);

        // The MeshRenderer component renders the quad as a blue outline
        // we only use this when Passthrough isn't visible
        quadOutline = projectionObject.GetComponent<MeshRenderer>();
        quadOutline.enabled = true;
    }

    void Update()
    {
        this.gameObject.transform.localScale += new Vector3(0.0f, add, 0.0f);
        if(this.gameObject.transform.localScale.y < 0.0f || this.gameObject.transform.localScale.y > 8){
            add *= -1;
        }
        /*
        // Hide object when A button is held, show it again when button is released, move it while held.
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
            passthroughLayer.RemoveSurfaceGeometry(projectionObject.gameObject);
            quadOutline.enabled = true;
        }
        if (OVRInput.Get(OVRInput.Button.One))
        {
            OVRInput.Controller controllingHand = OVRInput.Controller.RTouch;
            transform.position = OVRInput.GetLocalControllerPosition(controllingHand);
            transform.rotation = OVRInput.GetLocalControllerRotation(controllingHand);
        }
        if (OVRInput.GetUp(OVRInput.Button.One))
        {
            passthroughLayer.AddSurfaceGeometry(projectionObject.gameObject);
            quadOutline.enabled = false;
        }
        */
    }
}
