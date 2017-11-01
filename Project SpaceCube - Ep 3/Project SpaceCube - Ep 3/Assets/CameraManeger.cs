using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManeger : MonoBehaviour {

	// Use this for initialization
	void Start () {

        if(TheCamera == null)
        {
            // Do we have a camera component?
            TheCamera = GetComponent<Camera>();
        }
        if (TheCamera == null)
        {
            // Is their a main camera?
            TheCamera = Camera.main;
        }
        if (TheCamera == null)
        {
            Debug.LogError("Could not find a camera");
        }

        cameraRig = TheCamera.transform.parent;

    }

    public Camera TheCamera;
    private Transform cameraRig;
    private Vector3 lastMousePos;

    public float OrbitSensetivity = 10;
    public bool HoldToOrbit;

    public float ZoomMultiplayer = 2;
    public float minDistance = 2;
    public float maxDistance = 25;
    public bool InvertZoomDirection = false;
    public float PanSpeed = 0.25f;

    public bool isEditMode = true;

    public void SetModeEdit() { isEditMode = true; }
    public void SetModeFlight() { isEditMode = false; }


    void Update() {
        OrbitCamera();
        DollyCamera();
        PanCamera();
    }

    void PanCamera()
    {
        Vector3 input= new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        Vector3 actualChange = input * PanSpeed;

        actualChange = Quaternion.Euler(0, TheCamera.transform.rotation.eulerAngles.y, 0) * actualChange;

        Vector3 newPosition = cameraRig.transform.position + actualChange;

        cameraRig.transform.position = newPosition;
    }

    void DollyCamera()
    {
        float delta = -Input.GetAxis("Mouse ScrollWheel");
        if (InvertZoomDirection)
            delta = -delta;

        //Move camra forwed or backwards based on value of delta

        Vector3 actualChange = TheCamera.transform.localPosition * ZoomMultiplayer * delta;

        Vector3 newPosition = TheCamera.transform.localPosition + actualChange;

        newPosition = newPosition.normalized * Mathf.Clamp(newPosition.magnitude, minDistance, maxDistance);

        TheCamera.transform.localPosition = newPosition;

    }


    // Update is called once per frame
    void OrbitCamera () {
        //You can't orbit when in flight mode
        if(!isEditMode)  { return; }

        if( Input.GetMouseButtonDown(1) == true)
        {
            //The mouse was pressed down on this frame;
            lastMousePos = Input.mousePosition;

        }

        if ( Input.GetMouseButton(1) == true )
        {
            //we are curently holding down the right button

            // what is the current position of the mouse on the screen?
            Vector3 currentMousePos = Input.mousePosition;

            //In pixiles
            Vector3 mouseMovement = currentMousePos - lastMousePos;

            // Let's "Orbit" the camera rig with our actual camera object
            // When orbiting the distance from rig stays the same but 
            // the angle changes. Or we want to rotate the vector indicating 
            //relative postion of our camera from the rig

            Vector3 posRelativeToRig = TheCamera.transform.localPosition;

            Vector3 rotationAngles = mouseMovement / OrbitSensetivity;

            if (HoldToOrbit)
            {
                rotationAngles *= Time.deltaTime;
            }

            // TODO Fix me
            //Quaternion theOrbitalRotation = Quaternion.Euler(rotationAngles.y, rotationAngles.x, 0);

            //posRelativeToRig = theOrbitalRotation * posRelativeToRig;

            TheCamera.transform.RotateAround(cameraRig.position, TheCamera.transform.right, -rotationAngles.y);
            TheCamera.transform.RotateAround(cameraRig.position, TheCamera.transform.up, rotationAngles.x);

            //cameraRig.Rotate( theOrbitalRotation.eulerAngles, Space.self )

            //Make sure the camera is loocking at focal point (our rig)

            Quaternion lookRotation = Quaternion.LookRotation(-TheCamera.transform.localPosition);
            TheCamera.transform.rotation = lookRotation;

            //if you want to ceep the spining going without moving the mouse more don't do this!
            if (!HoldToOrbit)
            {
                lastMousePos = currentMousePos;
            }

        }

    }
}
