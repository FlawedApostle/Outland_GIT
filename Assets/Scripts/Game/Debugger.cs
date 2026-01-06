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
            // HEAD BONE
            if (Input.GetKeyDown(KeyCode.F9))
        {
            Debug.Log("[ROTATEBODY] MoveDir: " + DEBUG_RotateBodyMovement.Get_moveDir());
            Debug.Log("[ROTATEBODY]:[Camera] " + DEBUG_RotateBodyMovement.Get_Transform_CameraFPS_LocalRotation());
            Debug.Log("[ROTATEBODY] [Camera] [YAW] " + DEBUG_RotateBodyMovement.Get_Transform_CameraFPS_Yaw());
            Debug.Log("[ROTATEBODY] [BONE] [YAW]: " + DEBUG_RotateBodyMovement.Get_Transform_Bone_Head_Yaw());
            
            //Debug.Log("[ROTATEBODY] [BONE] [PITCH]: " + DEBUG_RotateBodyMovement.Get_Transform_Bone_Head_Pitch());
            //Debug.Log("[ROTATEBODY] [model-body] [YAW]: " + DEBUG_RotateBodyMovement.Get_Transform_Bone_Body_Yaw());


        }
            // ROTATE BODY
            if (Input.GetKeyDown(KeyCode.F10))
        {
            Debug.Log("[HEADBONE] [BONE] [YAW] " + DEBUG_HeadBoneMovement.Get_Transform_Bone_Head_Yaw());                                   // bone angle float 
            Debug.Log("[HEADBONE] [Camera] [YAW] " + DEBUG_HeadBoneMovement.Get_Transform_Camera_Yaw());
            Debug.Log("[HEADBONE] [Camera] " + DEBUG_HeadBoneMovement.Get_Transform_Camera_LocalRotation());
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            Debug.Log("[ROTATEBODY] [BONE] [YAW]: " + DEBUG_RotateBodyMovement.Get_Transform_Bone_Head_Yaw());
            Debug.Log("[HEADBONE] [BONE] [YAW] " + DEBUG_HeadBoneMovement.Get_Transform_Bone_Head_Yaw());
        }


        //Debug.Log("MoveDir: " + DEBUG_RotateBodyMovement.Get_moveDir() + " | targetYaw: " + DEBUG_RotateBodyMovement.Get_targetYaw());
        //Debug.Log("isWalking = " +  DEBUG_relativeMovement.Get_statusAnimator()); //+ " | magnitude = " + inputMovementMagnitude);       /// DEBUG
    }



}
