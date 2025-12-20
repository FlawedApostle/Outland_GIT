using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
// Connected in hierchey - YBot - Hips - Spine1 - Spine2 - Head(Script) - Top Head(Camera)
/// summary: This camera script is built around the idea of; you choose what you want to attach the cam to.
/// That is the transform head. The yaw and pitch input will be accessed through mouse input from the x and y axis
/// This input will direct the camera along with the transform movement of the object (head) - This in turn creates a [Camera relative system]
public class MouseLook : MonoBehaviour
{
    // MOUSE AXIS FOR CAMERA
    private float MouseAxis_X = 0f;
    private float MouseAxis_Y = 0f;
    private float GetMouseAxis_X() { return MouseAxis_X = Input.GetAxis("Mouse X"); }
    private float GetMouseAxis_Y() { return MouseAxis_Y = Input.GetAxis("Mouse Y"); }
    /// <summary>
    // ROTATION - HEAD
    /// My goal is to get a realistic head movment. I clamped at it first to restrict the horizontal axis as in a human cant turn their head like and owl !
    /// as of now its set to do 180 DEG in this case half a cirlce. - I dont find it practacle. However it works. SO im curious if I figure out the Body seperation ,
    /// Can I add a delay? as in if head turn reaches clamp limit turn body ? that body turn radius will be stored and then subtracted from the math clamp to have a realistic ' head & body ' rotation
    /// </summary> 
   float headUpperAngleLimit = 50f;                   /// Must be HIGHER than headLowerAngleLimit.   
   float headLowerAngleLimit = -50f;                  /// Must be LOWER than headUpperAngleLimit.
   float headRightTurnAngleLimit = -180f;
   float headLeftTurnAngleLimit = 180f;

    // TURN SPEED
    [SerializeField , Tooltip("Rotational Axis Speed")] float turnSpeed = 90f; // In inspector set to 200
    /// <summary>
    // Rotation - In following order Default , Head , Body
    /// </summary>
    // Default
    float yaw = 0f;
    float pitch = 0f;
    // Head
    float Headyaw => yaw;
    float Headpitch => pitch;
    // Default
    float clamp_yaw;
    float clamp_pitch;
    // Head
    float clamp_Headyaw;
    float clamp_Headpitch;

    // Functions Get Orintations

    /* Notes
    // Stores the orientations of the head and body when the game
    // started. We'll derive new orientations by combining these with our yaw and pitch.
    */
    /* Notes
    // A reference to the head object—the object to rotate up and down.
    // (The body is the current object, so we don't need a variable to store a reference to it.) 
    // Not exposed in the interface; instead,we'll figure out what to use by looking for a Camera child object at game start.
    */
    // HEAD STUFF
    public Transform head;
    public Quaternion headStartOrientation;
    public Quaternion headFinalOrientation;
    Quaternion headRotation;        // Head - Rotation
    Quaternion headRotRight;        // Head - Rotation
    Quaternion headRotUp;           // Head - Rotation
    // Camera Relative Movement - For moving the player based on which direction the camera is facing 
    public Vector3 HeadMovementPitch { get { Vector3 HeadForward = head.forward; HeadForward.y = 0f; return HeadForward.normalized; } }     // Get - Set function
    public Vector3 HeadMovementYaw { get { Vector3 HeadRight = head.right; HeadRight.y = 0f; return HeadRight.normalized; } }             // Get - Set function
    /// BODY STUFF
    //public Transform body;          // body
    public Quaternion bodyStartOrientation;
    Quaternion bodyRotation;
    Quaternion bodyRotationPitch;   // up
    Quaternion bodyRotationYaw;     // right
    public Quaternion finalBodyRotation;
    ///public Vector3 bodyMovementPitch { get { Vector3 BodyForward = body.forward; BodyForward.y = 0f; return BodyForward.normalized; } }     // Get - Set function
    ///public Vector3 bodyMovementYaw { get { Vector3 BodyRight = body.right; BodyRight.y = 0f; return BodyRight.normalized; } }             // Get - Set function 

    // GETTERS
    public Transform GetHead() { return head; }
    public Quaternion GetHeadRotation() { return headRotation; }
    public Quaternion GetHeadRotation(Quaternion q) { return headRotation = q; }
    ///public Transform GetBody() { return body; }
    public Quaternion GetBodyRotation() { return bodyRotation; }
    public Quaternion GetBodyStartOrientation() { return bodyStartOrientation; }
    public Quaternion GetFinalBodyRotation() { return finalBodyRotation; }

    private void Awake()
    {
        yaw = 0f;
        pitch = 0f;
    }

    // Perfom initial set up
    void Start()
    {
        cursorLockScreen(); // lock cursor
        // Cache the orientation of the body and head
        bodyStartOrientation = transform.localRotation;
        headStartOrientation = head.transform.localRotation;
    }




    /* Notes
    // Every time physics updates, update our movement. (We do this in FixedUpdate to keep pace with physically simulated objects.
    // If you won't be interacting with physics objects, you can do this in Update instead.)
    */
    void FixedUpdate()
    {

        // Head - Only gets the " Head " what ever you place in the transform under " Head " - This will be the associated movement 
        yaw += GetMouseAxis_X() * Time.deltaTime * turnSpeed;
        pitch += GetMouseAxis_Y() * Time.deltaTime * turnSpeed * -1;

        /* Notes
        // Read the current horizontal movement, and scale it based on the amount of time that's elapsed and the movement speed.
        //horizontal = Input.GetAxis("Mouse X") * Time.deltaTime * turnSpeed;
        //Same for vertical. -> multiply by neg to inverse rotation on Y axis
        //vertical = Input.GetAxis("Mouse Y") * Time.deltaTime * turnSpeed * -1;
        */
        /* Notes
        // Compute a rotation for the body by rotating around the y-axis
        // by the number of yaw degrees, and for the head around the
        // x-axis by the number of pitch degrees.
        */

        // NOTE - CLAMP PITCH SO THAT WE CAN'T LOOK DIRECTLY UP OR DOWN , LEFT OR RIGHT
        /// HEAD STUFF
        headRotUp = Quaternion.AngleAxis(Headyaw, Vector3.up);
        headRotRight = Quaternion.AngleAxis(Headpitch, Vector3.right);
        /// Clamp Head
        pitch = Mathf.Clamp(Headpitch, headLowerAngleLimit, headUpperAngleLimit);
        //Headyaw = Mathf.Clamp(Headyaw, headRightTurnAngleLimit, headLeftTurnAngleLimit);   - this clams rotation for head 'realistic' left & right head movement

        // ROTATIONS  NOTE - Create new rotations for the body and head by combining them with their start rotations
        /// HEAD ROTATION
        head.localRotation = headRotUp * headRotRight * headStartOrientation;                                   // this rotates and orbits the head (bone)


        /* TEST BLOCKS - HEADMOVEMENT
        ///transform.localRotation = bodyRotation * bodyStartOrientation;
        ///body.localRotation = bodyRotation * bodyStartOrientation;
        ///transform.localRotation = headRotUp * headRotRight * headStartOrientation;                         // this rotates and orbits the head (bone)
        */


    }


    private void cursorLockScreen()
    {
        // Lock and hide the cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public Vector3 SetYtoZero(Vector3 v)
    {
        v.y = 0;
        return v.normalized;
    }

}
