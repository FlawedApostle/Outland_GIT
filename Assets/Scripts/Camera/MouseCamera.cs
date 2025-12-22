using Unity.VisualScripting;
using UnityEngine;
// This takes the camera with the tag MainCamera. - to have muiltiple cams, a serialized transform will directly take the chosen camera
public class MouseCamera : MonoBehaviour
{
    ///[SerializeField] Transform MainCameraTransform;  - if there are multiple cams in the scene
    [SerializeField] float sensitivity = 2.0f;
    [SerializeField] float verticalLookLimit = 80f;
    [SerializeField] float horizontalLookLimit = -80f;

    // I am using my own set values for better control, frame to frame management
    private float _rotationX = 0f; // Left/Right (Yaw)
    private float _rotationY = 0f; // Up/Down (Pitch)

    // Clamp
    float min, max, clamp_X, clamp_Y;
    private float ClampAxisRotation_Y(float rotY , float vertLimit)
    {
        min = -Mathf.Abs(vertLimit);
        max = Mathf.Abs(vertLimit);
        clamp_Y = Mathf.Clamp(rotY, min, max);
        return clamp_Y;
    }
    private float ClampAxisRotation_X(float rotX, float vertLimit)
    {
        min = -Mathf.Abs(vertLimit);
        max = Mathf.Abs(vertLimit);
        clamp_X = Mathf.Clamp(rotX, min, max);
        return clamp_X;
    }
    public void PrintClampAxisRotation_Y(float Clampvalue) { Debug.Log("[MouseCamera DEBUG] Current Clamp Y Axis " + Clampvalue); }
    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    // Camera Position
    public void CamCoordinateLocation()
    {
        Debug.Log("[MouseCamera DEBUG] Camera Coordinate Location: (" + _rotationX + " , " + _rotationY + ")" );
    }


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
        _rotationX = 0f;
        _rotationY = 0f;
    }
    void Start()
    {
        // Lock the cursor to the center of the screen for better control
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {

        // 1. Get the change in mouse position since last frame
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // 2. Accumulate the values
        _rotationX += mouseX;
        _rotationY -= mouseY; // Inverted so moving mouse up looks up
        CamCoordinateLocation();    // DEBUG

        // 3. Clamp the vertical look to prevent the camera from flipping over 
        _rotationY = Mathf.Clamp(_rotationY, -verticalLookLimit, verticalLookLimit); // Axis Y 
         //Debug.Log("[MouseCamera] Axis Y Clamp " + _rotationY);

        // 4. Apply the rotation directly to the camera
        transform.eulerAngles = new Vector3(_rotationY, _rotationX, 0f);
        PrintClampAxisRotation_Y(_rotationY); // DEBUG - print the y axis clamp

        /// We must set if the clamp reaches its limit the clamp reverts back to zero to then continue rotation in a realistic manner

        /// Get the rotation as a Quaternion (best for math/interpolation)
        //Quaternion camRotation = Camera.main.transform.rotation;
        //Get the rotation as Euler angles (Vector3 - X, Y, Z)
        //Vector3 camEulerAngles = Camera.main.transform.eulerAngles;

        //    // Example: Make this object face the same way as the camera's forward direction Sets forward direction
        //transform.forward = Camera.main.transform.forward;

        //    //  Or use the rotation directly Sets full rotation - this is used for Bone Movement
        //    // use camRotation as a global object for headBone to get exact movement
        //transform.rotation = camRotation;

        ///Debug.Log("[MouseCamera DEBUG] Camera Rotation (Euler): " + transform.eulerAngles);
    }


}
