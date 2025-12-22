using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XR;
// Relative Camera Movement. Player will follow facing the direction of the camera (head)

public class RelativeMovement : MonoBehaviour
{
    // I am calling an object of the MouseCamera script , Which has a [Serialized Field] Transform of the INTENDED FPS camera
    // Inside RelativeMovement I am using a public [Serialized Field] Transform of which the user places the SAME INTENDED FPS camera into
    // When RelativeMovement awakes the Transform of the MouseCamera script is called, and is set to the Transform of the RelativeMovement script
    // In this case we have absolute controll ? or is it a reference ? - here is where I get lost a little bit,
    // I do not want to be able to change the value here in RelativeMovement, rather I want A COPY of all values to which I can compare against other values
    public MouseCamera mouseCamera;
    [Tooltip("Ensure MATCHING FPS CAMERAS - Taking a refernce of the Camera from MouseCamera Script")] Transform MouseCamera_CAMERA;
    //[SerializeField] public Transform Camera;
    [SerializeField] float speed = 5f;

    // CAMERA Direction (relative)
    /*IMPORTANT NOTE
    // Taking the transform directly from MouseCamera, and then creating a copy (Safe)
    // Unity axis works as follows do the 3D hand gun axis on YOUR RIGHT HAND if you look you will see the following;
    // Y axis is up, perpendicular to Y is the X axis (to the right). Finally Z axis points forward (index finger) 
    */
    Vector3 camForward , camRight;
    Quaternion camOrientation;
    /// Relative direction will take the coordinates of the Camera direction in relation
    public void RelativeCameraMovementDirection_Vector()
    {
        Debug.Log($"[RelativeMovement DEBUG] Cam Relative Forward: {camForward} | Cam Relative Right: {camRight}");
    }
    public void RelativeCameraMovementDirection_Quaternion()
    {
        Debug.Log($"[RelativeMovement DEBUG] Full Camera Orientation (Quaternion): {camOrientation}");
    }
    

    ///  I now need to compare the direction vectors  !

    private void Start()
    {
        if (mouseCamera != null)
        {
            MouseCamera_CAMERA = mouseCamera.MainCameraTransform;
        }
            
    }

    void Update()
    {

        //////////////////////////////////////// GETTING CAMERA ORIENTATION \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /* IMPORTANT NOTE: This is using the TAG system, Camera Must have MainCamera Tag associated
        //Vector3 camForward = Camera.main.transform.forward;
        //Vector3 camRight = Camera.main.transform.right;
        */
        // 1. Get raw camera directions without an Inspector slot - 
        // Taking the transform directly from MouseCamera, and then creating a copy (Safe)
        // Unity axis works as follows do the 3D hand gun axis on YOUR RIGHT HAND if you look you will see the following;
        // Y axis is up, perpendicular to Y is the X axis (to the right). Finally Z axis points forward (index finger)
        camForward = MouseCamera_CAMERA.transform.forward;
        camRight = MouseCamera_CAMERA.transform.right;
        // 2. Flatten the vectors so you don't move into the ground
        camForward.y = 0;
        camRight.y = 0;
        // 3. Re-normalize to ensure the length is exactly 1
        camForward.Normalize();
        camRight.Normalize();
        // 4. DEBUG output to show current movement directions
        RelativeCameraMovementDirection_Vector();                                          
        // Bonus: Show the orientation as a Quaternion if needed
        camOrientation = MouseCamera_CAMERA.transform.rotation;                     // Referencing the transform camera input
        //Quaternion camOrientation = Camera.main.transform.rotation;               // OG
        //Debug.Log($"Full Camera Orientation (Quaternion): {camOrientation}");     // DEBUG output Quaternion orientation
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //////////////////////////////////////// GETTING WASD or JOYSTICK INPUT \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        // 1. Get input from WASD or Joystick
        float horizontal = UnityEngine.Input.GetAxis("Horizontal");
        float vertical = UnityEngine.Input.GetAxis("Vertical");

        // 2. Get and "Flatten" camera directions (ignore tilt)
        //camForward = Camera.main.transform.forward;     // OG
        //camRight = Camera.main.transform.right;         // OG

        camForward = MouseCamera_CAMERA.transform.forward;
        camRight = MouseCamera_CAMERA.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // 3. Combine into a final movement vector
        Vector3 moveDirection = (camForward * vertical) + (camRight * horizontal);

        // 4. Apply movement (using normalization to keep speed consistent)
        if (moveDirection.magnitude > 0.1f)
        {
            // Move the object
            transform.position += moveDirection.normalized * speed * Time.deltaTime;

            // Debug the final direction vector
            ///Debug.Log($"[RelativeMovement DEBUG] Moving in relative direction: {moveDirection.normalized}");
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    
    }

}       // !#END of Class RelativeMovement
