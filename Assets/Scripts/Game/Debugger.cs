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
            Debug.Log("[HEADBONE] - HEAD");
            Debug.Log("[HEADBONE] [model-head]  [LOCAL]: " + DEBUG_HeadBoneMovement.Get_Transform_Bone_Head_LocalRotation());                          // bone local rot
            Debug.Log("[HEADBONE] [model-head]  [YAW]: "         + DEBUG_HeadBoneMovement.Get_Transform_Bone_Head_Yaw());                                   // bone angle float 
            Debug.Log("[HEADBONE] - CAM");
            Debug.Log("[HEADBONE] [Camera] [LOCAL]: "       + DEBUG_HeadBoneMovement.Get_Transform_Camera_LocalRotation());                  // cam local rot
            Debug.Log("[HEADBONE] [Camera] [YAW]: "         + DEBUG_HeadBoneMovement.Get_Transform_Camera_Yaw());                            // cam euler.y

               

        }
            // ROTATE BODY
            if (Input.GetKeyDown(KeyCode.F10))
        {
            //Debug.Log("F10 pressed!");
            Debug.Log("[ROTATEBODY] - HEAD");
            Debug.Log("[ROTATEBODY] [model-head] [LOCAL]: "      + DEBUG_RotateBodyMovement.Get_Transform_HeadBone_LocalRotation());
            Debug.Log("[ROTATEBODY] [model-head] [YAW]: "        + DEBUG_RotateBodyMovement.Get_Transform_HeadBone_Yaw());
            Debug.Log("[ROTATEBODY] [model-head] [PITCH]: "      + DEBUG_RotateBodyMovement.Get_Transform_HeadBone_Pitch());
            Debug.Log("[ROTATEBODY] - CAM");
            Debug.Log("[ROTATEBODY] [Camera] [LOCAL]: "     + DEBUG_RotateBodyMovement.Get_Transform_CameraFPS_LocalRotation());
            Debug.Log("[ROTATEBODY] [Camera] [YAW]: "       + DEBUG_RotateBodyMovement.Get_Transform_CameraFPS_Yaw());
            Debug.Log("[ROTATEBODY] - MOVE DIRECTION");
            Debug.Log("[ROTATEBODY] MoveDir: "              + DEBUG_RotateBodyMovement.Get_moveDir());
        }


        //Debug.Log("MoveDir: " + DEBUG_RotateBodyMovement.Get_moveDir() + " | targetYaw: " + DEBUG_RotateBodyMovement.Get_targetYaw());
        //Debug.Log("isWalking = " +  DEBUG_relativeMovement.Get_statusAnimator()); //+ " | magnitude = " + inputMovementMagnitude);       /// DEBUG
    }



}
