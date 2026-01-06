using UnityEngine;
using UnityEngine.UIElements;

public class Debuger : MonoBehaviour
{
    [Header("SCRIPTS: Rotate Body Reference")]
    [SerializeField]// Tooltip("Reference to the RelativeMovement script for movement direction")]
    private RotateBodyMovement DEBUG_RotateBodyMovement;

    [Header("SCRIPTS: Relative Movement Reference")]
    [SerializeField, Tooltip("Reference to the RelativeMovement script for movement direction")]
    private RelativeMovement DEBUG_RelativeMovement;

    [Header("SCRIPTS: Head Bone Movement Reference")]
    [SerializeField, Tooltip("Reference to the RelativeMovement script for movement direction")]
    private HeadBoneMovement DEBUG_HeadBoneMovement;
    public float NULLCHECK_HeadBoneMovement()      // DEBUG
    {
        if (DEBUG_HeadBoneMovement == null) { Debug.LogError("DEBUG_HeadBoneMovement is NULL!"); }
        return 0f;
    }

    [Header("SCRIPTS: Animator Script Reference")]
    //[SerializeField, Tooltip("Reference to the RelativeMovement script for movement direction")]
    private Animator DEBUG_animator;


    private void Start()
    {
        NULLCHECK_HeadBoneMovement();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {

        }
            // HEAD BONE
            if (Input.GetKeyDown(KeyCode.F9))
        {
            //Debug.Log("F9 pressed!");
            Debug.Log("HeadBone [model]  HeadBone Local Rotation [DIRECTION]: " + DEBUG_HeadBoneMovement.Get_HeadBone_LocalRotation());                          // bone local rot
            Debug.Log("HeadBone [model]  Rotation y axis [YAW]: "               + DEBUG_HeadBoneMovement.Get_HeadBone_Yaw());                                   // bone angle float 
            Debug.Log("HeadBone [Camera] Local Rotation: "                      + DEBUG_HeadBoneMovement.Get_CameraTransform_LocalRotation());                  // cam local rot
            Debug.Log("HeadBone [Camera] Transform y axis [YAW]: "              + DEBUG_HeadBoneMovement.Get_CameraTransform_Yaw());                            // cam euler.y

               

        }
            // ROTATE BODY
            if (Input.GetKeyDown(KeyCode.F10))
        {
            //Debug.Log("F10 pressed!");
            Debug.Log("RotateBody Movement [Camera] local rotation: "   + DEBUG_RotateBodyMovement.Get_CameraFPS_LocalRotation());
            Debug.Log("RotateBody Movement [Camera] y axis [YAW]: "     + DEBUG_RotateBodyMovement.Get_CameraFPS_Yaw());  
            Debug.Log("RotateBody Rotation [model] [YAW]: "             + DEBUG_RotateBodyMovement.Get_HeadYaw());
            Debug.Log("RotateBody Rotation [model] [PITCH]: "             + DEBUG_RotateBodyMovement.Get__headPitch());
        }


        //Debug.Log("MoveDir: " + DEBUG_RotateBodyMovement.Get_moveDir() + " | targetYaw: " + DEBUG_RotateBodyMovement.Get_targetYaw());
        //Debug.Log("isWalking = " +  DEBUG_relativeMovement.Get_statusAnimator()); //+ " | magnitude = " + inputMovementMagnitude);       /// DEBUG
    }



}
