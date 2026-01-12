using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XR;
/* NOTE: 
 * THIS IS BUILT BY SAMUEL FEARNLEY WHOM IS THE SOLE CREATOR AND PROPIETER OF SUCH SOFTWARE
 * IF YOU WISH TO USE THE CODE PLEASE REACH OUT TO EITHER MY DISCORD, OR GITHUB ACCOUNTS - IM REALLY NOT PICKY, I JUST WANT TO BE SURE NOTHING IS USED MALICOUSLY 
 */
/* These notes are for me, I dont care what others think. I have to keep track of what I am doing.
* Relative Camera Movement. Player will follow facing the direction of the camera (head)
* magnitude depends on movement  inputMagnitude > 0.01f
* Unity uses a left hand 3D Coord system. [LEFT/RIGHT = X AXIS | UP/DOWN = Y AXIS | FORWARD/BACK = Z AXIS] 
* inputMovementVector  - is the raw input data from the x(horizontal) , y(vertical) axis of the mouse input
* VERTICAL_VELOCITY: Move the character if the player is moving, OR the player is not on the ground, OR the player is going upward.
* (inputMovementMagnitude > 0.01f || !isGrounded || verticalVelocity > 0f) “This condition decides whether we should apply movement, no matter if the player is walking, in the air, or in the first frame of a jump.”
* */

/// <summary>
///  TO DO
///  ADD A CHARACTER CONTROLLER FOR MOVEMENT
///  FIX MOVMEMNT IF/ELSE SCRIPT
/// </summary>

public class RelativeMovement : MonoBehaviour
{
    /// Animation Player Controller calling it from inspector
    [SerializeField] private Animator anim;
    /* NOTES: MOUSECAM
    // I am calling an object of the MouseCamera script , Which has a [Serialized Field] Transform of the INTENDED FPS camera
    // Inside RelativeMovement I am using a public [Serialized Field] Transform of which the user places the SAME INTENDED FPS camera into
    // When RelativeMovement awakes the Transform of the MouseCamera script is called, and is set to the Transform of the RelativeMovement script
    // In this case we have absolute controll ? or is it a reference ? - here is where I get lost a little bit,
    // I do not want to be able to change the value here in RelativeMovement, rather I want A COPY of all values to which I can compare against other values
    */
    [Header("Camera - FPS")]
    [SerializeField][Tooltip("Ensure MATCHING FPS CAMERAS - Taking a refernce of the Camera from MouseCamera Script")] 
    Transform MouseCamera_CAMERA;
    /* NOTES: MOUSECAM
    // Taking the transform directly from MouseCamera, and then creating a copy (Safe)
    // Unity axis works as follows do the 3D hand gun axis on YOUR RIGHT HAND if you look you will see the following;
    // Y axis is up, perpendicular to Y is the X axis (to the right). Finally Z axis points forward (index finger) 
    */
    Vector3 camForward, camRight;
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
    /// Character Controller
    [SerializeField][Tooltip("Character Controller is Needed for Movement Input")] CharacterController characterController;

    // Input Controlls
    private float horizontal;
    private float vertical;
    [Header("Player Attributes")]
    [SerializeField] float sprintSpeed = 6f;            /// the speed in which player can move, units per second
    [SerializeField] float moveSpeed = 4;
    [SerializeField] float jumpHeight = 2f;
    [SerializeField] float gravity = 9.8f;
    /// Build a slider - The degree to which we can control our movement while in midair.
    [Range(0, 10), SerializeField, Tooltip("The degree to which we can control our movement while in midair")] float airControl = 5;

    /// MOVEMENT DIRECTION
    Vector3 horizontalMove = Vector3.zero;
    Vector3 moveDirection = Vector3.zero;
    Vector3 finalMove = Vector3.zero;
    Vector3 input;
    private bool isGrounded;
    private float verticalVelocity = 0f;
    private float currentSpeed;
    public Vector3 GetMoveDirection() {
        return moveDirection;
    }


    [SerializeField , Tooltip("Toggle Animation on and off - Debugging")] bool AnimationToggle = true;

    private void Start()
    {
        currentSpeed = moveSpeed;
        anim.enabled = AnimationToggle;                   // debugger - turn off animation for camera testing
    }

    void Update()
    {
        //////////////////////////////////////// GETTING CAMERA ORIENTATION \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /* NOTES - MouseCamera_CAMERA explained
        // IMPORTANT NOTE: This is using the TAG system, Camera Must have MainCamera Tag associated
        // Vector3 camForward = Camera.main.transform.forward;
        // Vector3 camRight = Camera.main.transform.right; 
        // 1. Get raw camera directions without an Inspector slot - 
        // Taking the transform directly from MouseCamera, and then creating a copy (Safe)
        // Unity axis works as follows DO THE 3D hand gun axis on YOUR LEFT HAND if you look you will see the following;
        // Y axis is up, perpendicular to Y is the X axis (to the right). Finally Z axis points forward (index finger)
        */
        camForward = MouseCamera_CAMERA.transform.forward;                      // W/S forwad / backward - think of it on a 3D axis (finger axis left hand)
        camRight = MouseCamera_CAMERA.transform.right;                          // A/D left / right
        // 2. Flatten the vectors so you don't move into the ground
        camForward.y = 0;
        camRight.y = 0;
        // 3. Re-normalize to ensure the length is exactly 1
        camForward.Normalize();
        camRight.Normalize();
        // Show the orientation as a Quaternion if needed                           //Quaternion camOrientation = Camera.main.transform.rotation;               // OG
        camOrientation = MouseCamera_CAMERA.transform.rotation;                     // Referencing the transform camera input
        //Debug.Log($"Full Camera Orientation (Quaternion): {camOrientation}");     // DEBUG output Quaternion orientation
        //////////////////////////////////////// GETTING WASD or JOYSTICK INPUT \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        /// 1. Get input from WASD or Joystick
        horizontal = UnityEngine.Input.GetAxis("Horizontal");           /// A/D left, right
        vertical = UnityEngine.Input.GetAxis("Vertical");               /// W/S Up, down
        // 2. Apply horizontal movement (camera‑relative)
        moveDirection = (camForward * vertical) + (camRight * horizontal);  /// yaw * input axis + pitch * input axis 
        isGrounded = characterController.isGrounded;      
        //moveDirection *= moveSpeed;         //test line
        
        //////////////////////////////////////// MOVEMENT BLOCK \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
        // 4. movement vector  [left, right , up , down]
        Vector2 inputMovementVector = new Vector2(horizontal, vertical);

        float inputMovementMagnitude = inputMovementVector.magnitude;                    // getting the length of the vector


        // Debug Yaw and Pitch coordinats
        // I need to get he faciing direction ofthe camera Yaw - debug and gather data on that
        /*
        if(UnityEngine.Input.GetKeyDown(KeyCode.F2))
        {
            //Debug.Log("[Relative Movement DEBUG] horizontal coords: " + horizontal);            // input - 'when moving'
            Debug.Log("[Relative Movement DEBUG] " + "\n" +

                "INPUT COORDS " + "\n" +
                "moveDirection coords: " + moveDirection + "\n" +
                "vertical and horizonatal movement (raw): " + vertical + horizontal + "\n" +


                "MOUSE CAMERA COORDS " +
                "mouseCamera coords: " + camForward + " , " + camRight + "\n" +
                "mouseCamera Yaw (eulerAngles.y): " + MouseCamera_CAMERA.transform.eulerAngles.y);

            //Debug.Log("[Relative Movement DEBUG] mouseCamera eularAngle y: " + MouseCamera_CAMERA.transform.eulerAngles.y);
        }
        */
        // Decide current speed: base moveSpeed or sprintSpeed
        currentSpeed = moveSpeed;
        bool isMoving = inputMovementMagnitude > 0.01f && isGrounded;
        bool isSprinting = isMoving && UnityEngine.Input.GetButton("Sprint");
        if (isGrounded && inputMovementMagnitude > 0.01f)
        {
            if (UnityEngine.Input.GetButton("Sprint"))
            {
                isSprinting = true;
                Debug.Log("Sprint Pressed");
                currentSpeed = sprintSpeed;
            }
        }



        anim.SetBool("isWalking", isMoving);
        anim.SetFloat("speed", currentSpeed);
        //anim.SetBool("isSprinting", isSprinting);


        PrintTools.Print(currentSpeed, "blue", "Current Speed");

        //currentSpeed = new Vector3(characterController.velocity.x, 0, characterController.velocity.z).magnitude;
        //anim.SetFloat("speed", currentSpeed);


        // Animate walking
        //anim.enabled = false;                                       /// TURNED OFF ANIMATION
        //anim.SetBool("isWalking", inputMovementMagnitude > 0.01f && isGrounded);            // animate
        //Debug.Log("isWalking = " + anim.GetBool("isWalking") + " | magnitude = " + inputMovementMagnitude);       /// DEBUG

        // Handle jump and gravity
        if (isGrounded)
        {
            // Only allow jump if grounded and there's some input or not (design choice)
            if (UnityEngine.Input.GetButtonDown("Jump")) 
            {
                Debug.Log("Jump Pressed");
                verticalVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);        /// function ?
            }
            else if (verticalVelocity < 0f) 
            {
                // Slight negative to keep controller grounded
                verticalVelocity = -1f;
            }
        }
        else // In the Air
        {
            /// In the air: apply gravity over time
            verticalVelocity -= gravity * Time.deltaTime;
            /// blend moveDirection towards input, without killing Y
            input = moveDirection; // keep variable in use
            moveDirection = Vector3.Lerp(moveDirection, input, airControl * Time.deltaTime);
        }
        // MOVING CONDITIONS
        ///  Build horizontal movement (camera-relative) [CREATING MOVEMENT VECTOR]
        horizontalMove = moveDirection.normalized * currentSpeed;
        ///  Combine horizontal + vertical into finalMove - [LEFT HAND 3D AXIS] (this line is here to ensure sprint works - (for now))
        finalMove = new Vector3(horizontalMove.x, verticalVelocity, horizontalMove.z);
        /// “This condition decides WHETHER we should apply movement, no matter if the player is walking, in the air, or in the first frame of a jump.”
        if (inputMovementMagnitude > 0.01f || !isGrounded || verticalVelocity > 0f)
        {
              characterController.Move(finalMove * Time.deltaTime);
        }
    }

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




}       //!#END 

