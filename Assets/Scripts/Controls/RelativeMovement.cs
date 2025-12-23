using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XR;
// Relative Camera Movement. Player will follow facing the direction of the camera (head)

/// <summary>
///  TO DO
///  ADD A CHARACTER CONTROLLER FOR MOVEMENT
///  FIX MOVMEMNT IF/ELSE SCRIPT
/// </summary>

public class RelativeMovement : MonoBehaviour
{
    // I am calling an object of the MouseCamera script , Which has a [Serialized Field] Transform of the INTENDED FPS camera
    // Inside RelativeMovement I am using a public [Serialized Field] Transform of which the user places the SAME INTENDED FPS camera into
    // When RelativeMovement awakes the Transform of the MouseCamera script is called, and is set to the Transform of the RelativeMovement script
    // In this case we have absolute controll ? or is it a reference ? - here is where I get lost a little bit,
    // I do not want to be able to change the value here in RelativeMovement, rather I want A COPY of all values to which I can compare against other values
    [Header("Camera - FPS")]
    [SerializeField][Tooltip("Ensure MATCHING FPS CAMERAS - Taking a refernce of the Camera from MouseCamera Script")] Transform MouseCamera_CAMERA;
   
    [Header("Player Attributes")]
    [SerializeField] float speed = 5f;
    [SerializeField] float gravity = 9.8f;
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float sprintSpeed = 5f;
    // The degree to which we can control our movement while in midair.
    [Range(0, 10), SerializeField, Tooltip("The degree to which we can control our movement while in midair")] float airControl = 5;
    
    /// Character Controller
    [SerializeField] [Tooltip("Character Controller is Needed for Movement Input")]CharacterController characterController;
    public void Debug_characterController(CharacterController charactercontroller)
    {
        charactercontroller = characterController;
        if (characterController == null)
        {
            Debug.Log("No Character Controller , One has now been set");
            characterController = GetComponent<CharacterController>();
        }
        Debug.Log("Character Controller set");
    }
    // MOVEMENT DIRECTION
    Vector3 moveDirection = Vector3.zero;
    Vector3 input;

    // Input Controlls
    float horizontal , vertical;

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



    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        Debug_characterController(characterController);
    }

    void Update()
    {

        //////////////////////////////////////// GETTING CAMERA ORIENTATION \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /* IMPORTANT NOTE: This is using the TAG system, Camera Must have MainCamera Tag associated
        //Vector3 camForward = Camera.main.transform.forward;
        //Vector3 camRight = Camera.main.transform.right;
        */
        /* NOTES - MouseCamera_CAMERA explained
        // 1. Get raw camera directions without an Inspector slot - 
        // Taking the transform directly from MouseCamera, and then creating a copy (Safe)
        // Unity axis works as follows DO THE 3D hand gun axis on YOUR LEFT HAND if you look you will see the following;
        // Y axis is up, perpendicular to Y is the X axis (to the right). Finally Z axis points forward (index finger)
        */
        camForward = MouseCamera_CAMERA.transform.forward;
        camRight = MouseCamera_CAMERA.transform.right;
        // 2. Flatten the vectors so you don't move into the ground
        camForward.y = 0;
        camRight.y = 0;
        // 3. Re-normalize to ensure the length is exactly 1
        camForward.Normalize();
        camRight.Normalize();

        // Show the orientation as a Quaternion if needed                           //Quaternion camOrientation = Camera.main.transform.rotation;               // OG
        camOrientation = MouseCamera_CAMERA.transform.rotation;                     // Referencing the transform camera input
        //Debug.Log($"Full Camera Orientation (Quaternion): {camOrientation}");     // DEBUG output Quaternion orientation
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        //////////////////////////////////////// GETTING WASD or JOYSTICK INPUT \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        // 1. Get input from WASD or Joystick
        horizontal = UnityEngine.Input.GetAxis("Horizontal");
        vertical = UnityEngine.Input.GetAxis("Vertical");

        // 3. Combine into a final movement vector
        moveDirection = (camForward * vertical) + (camRight * horizontal);

        //////////////////////////////////////// MOVEMENT BLOCK \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        // 4. Apply movement (using normalization to keep speed consistent)
        if (moveDirection.magnitude > 0.1f)
        {
            // Move the object
            transform.position += moveDirection.normalized * speed * Time.deltaTime;


            // Debug the final direction vector
            ///Debug.Log($"[RelativeMovement DEBUG] Moving in relative direction: {moveDirection.normalized}");
        }
        // DEBUGGING RelativeMovement DIRECTION COORDS
        if (UnityEngine.Input.GetKeyDown(KeyCode.F2))       
        {
            RelativeCameraMovementDirection_Vector();
        }

        if (UnityEngine.Input.GetButton("Jump"))
        {
            Debug.Log("Jump Pressed");
            moveDirection.y = Mathf.Sqrt(2 * gravity * jumpHeight);
        }

        if(UnityEngine.Input.GetButton("Sprint"))
        {
            Debug.Log("Sprint Pressed");
            moveDirection *= sprintSpeed;
        }

        else
        {
            moveDirection.y = 0;
            input.y = moveDirection.y;
            moveDirection = Vector3.Lerp(moveDirection, input, airControl * Time.deltaTime);
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    
    }

}       // !#END of Class RelativeMovement
