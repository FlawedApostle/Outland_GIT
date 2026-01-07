using System;
using Unity.VisualScripting;
using UnityEngine;
// This takes the camera with the tag MainCamera. - to have muiltiple cams, a serialized transform will directly take the chosen camera
public class MouseCamera : MonoBehaviour
{
    public Transform MainCameraTransform;  // FPS cam in scene
    [SerializeField] float sensitivity = 2.0f;
    [SerializeField] float verticalLookLimit = 80f;
    [SerializeField] float horizontalLookLimit = -80f;          /// not using currently, may implement later as a layer for torso movemnt

    // I am using my own set values for better control, frame to frame management
    private float _rotationX = 0f; // (Yaw)    Left/Right 
    private float _rotationY = 0f; // (Pitch)  Up/Down
    private float mouseX = 0f;
    private float mouseY = 0f;
    private Quaternion _mouseXYQuat;
    public Quaternion Get_MouseXYQuat()
    { return _mouseXYQuat;  }

    public Transform Get_MainCameraTransform()  { return MainCameraTransform;  }
 
    ////////////////////////////////////////// Camera Euler Angles - can only use in awake, so dont call for now. It cannot call an instance when it is null at scene start
    //Vector3 _camEulerAngles = Camera.main.transform.eulerAngles;
    //float yaw = Camera.main.transform.eulerAngles.y; 
    //float pitch = Camera.main.transform.eulerAngles.x;
    //float GetYaw() { return yaw; }
    //float GetPitch() { return pitch; }
    //Vector3 GetCamEulerAngles() { return _camEulerAngles; }
    //////////////////////////////////////////////////////////
    private void Awake()
    {
        if (MainCameraTransform == null)
        MainCameraTransform = GetComponent<Transform>();
    }

    RelativeMovement relativeMovement;
    void Start()
    {   
        // Static class CursorTools
        CursorTools.Lock_Cursor();
    }

    void Update()
    {

        // 1. Get the change in mouse position since last frame
        mouseX = Input.GetAxis("Mouse X") * sensitivity;
        mouseY = Input.GetAxis("Mouse Y") * sensitivity;
        // 2. Accumulate the values
        _rotationX += mouseX;
        _rotationY -= mouseY; /// (-=) Inverted so moving mouse up looks up
        // 3. Clamp the vertical look to prevent the camera from flipping over 
        _rotationY = Mathf.Clamp(_rotationY, -verticalLookLimit, verticalLookLimit); // Axis Y 
        // 4. Apply the rotation directly to the camera
        _mouseXYQuat = Quaternion.Euler(_rotationY, _rotationX, 0f);
        MainCameraTransform.rotation = _mouseXYQuat;
        //MainCameraTransform.eulerAngles = new Vector3(_rotationY, _rotationX, 0f);  // Applying it to a set transform to ensure data integrety


    }




}
