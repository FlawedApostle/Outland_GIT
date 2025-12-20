using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XR;
/// Implements character controller movement
/// Summary: The idea of this script is to to [move] the player character - basic movement - Jump - run - crouch - hide
public class Movement : MonoBehaviour
{
    //Timer
    float stopWatch = 0f;
    // INPUT
    private float horizontalInput;
    private float verticalInput;
    /// the speed in which player can move, units per second
    [SerializeField] float moveSpeed = 6;
    /// Jump Height
    [SerializeField] float jumpHeight = 2;
    /// Rate of vertical speed will be reduced, in units per second  -> 9.8 earth gravity
    [SerializeField] float gravity = 9.8f;
    // The degree to which we can control our movement while in midair.
    [Range(0, 10), SerializeField ,Tooltip("The degree to which we can control our movement while in midair")] float airControl = 5;
    // Our current movement direction. If we're on the ground, we have
    // direct control over it, but if we're in the air, we only have partial control over it.
    Vector3 moveDirection = Vector3.zero;
    /// A cached reference to the character controller
    CharacterController controller;
    [Header("Rotation"), Tooltip("How Fast to Rotate the Camera for Relative Player Movement")]
    //[SerializeField] float rotationSpeed = 5f;

    // RUN
    [Header("Run Speed")]
    [SerializeField] float sprintSpeed = 5;
    // MouseLook - Get the scene camera
    [SerializeField] public Transform Cam;
    [SerializeField] public MouseLook mouseLook;        // calling MouseLook script NEEDED for Realative Camera Movement

    // Moving i have to send out if the player is moving by gathering its directional data
    Vector3 input;
    public Vector3 GetInput()
    {
        return input;
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();       /// Player controller
    }

    void FixedUpdate()
    {

        // Get input from keyboard (WASD/Arrows) ( implement later the UNITY  input system )
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        /* TEST BLOCK
        // Get Cam relative direction - Flatten the vectors to the XZ plane for 3D ground movement (prevents character from moving up/down when looking at horizon)
        //Vector3 CamForward = Cam.forward;
        //Vector3 CamRight = Cam.right;
        //CamForward.y = 0f;
        //CamRight.y = 0f;
        //CamForward.Normalize();
        //CamRight.Normalize();
        */
        /* Notes
        // The input vector describes the user's desired local-space movement; if we're on the ground, this will immediately
        // become our movement, but if we're in the air, we'll interpolate between our current movement and this vector, to simulate momentum.
        */
        input = (mouseLook.HeadMovementPitch * verticalInput + mouseLook.HeadMovementYaw * horizontalInput).normalized;
        // Multiply this movement by our desired movement speed
        input *= moveSpeed;
        // The controller's Move method uses world-space directions, so we need to convert this direction to world space
        input = transform.TransformDirection(input);


        // Is the controller's bottommost point touching the ground?
        if (controller.isGrounded)
        {
            // Figure out how much movement we want to apply in local space.
            moveDirection = input;


            
            if (Input.GetButton("Jump"))        /// JUMP
            {
                // Calculate the amount of upward speed we need, considering that we add moveDirection.y to our height every frame, and we reduce moveDirection.y by gravity every frame.
                moveDirection.y = Mathf.Sqrt(2 * gravity * jumpHeight);
                Debug.Log("Made it to JMP !");
            }
            else if (Input.GetButton("Sprint"))       /// SPRINT
            {
                moveDirection *= sprintSpeed;
            }
            else
            {
                /// We're on the ground, but not jumping. Set our downward movement to 0 (otherwise, because we're continuously reducing our y-movement, if we walk off a ledge, we'd suddenly have a huge amount of downward momentum).
                moveDirection.y = 0;
            }
        }
        else
        {
            /// Slowly bring our movement toward the user's desired input, but preserve our current y-direction (so that the arc of the jump is preserved)
            input.y = moveDirection.y;
            moveDirection = Vector3.Lerp(moveDirection, input, airControl * Time.deltaTime);
        }

        // Gravity - Bring Down movment
        moveDirection.y -= gravity * Time.deltaTime;

        /* Notes
        // Move the controller. The controller will refuse to move into
        // other colliders, which means that we won't clip through the
        // ground or other colliders. (However, this doesn't stop other
        // colliders from moving into us. For that, we'd need to detect
        // when we're overlapping another collider, and move away from
        // them. We'll cover this in another recipe!)
        */
        controller.Move(moveDirection * Time.deltaTime);
    }


}