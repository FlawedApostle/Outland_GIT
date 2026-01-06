using UnityEngine;

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
            //Debug.Log("F9 pressed!");
            Debug.Log("HeadBone Rotation Direction: " + DEBUG_HeadBoneMovement.Get_HeadBone_Rotation() + " | HeadBone Rotation Yaw: " + DEBUG_HeadBoneMovement.Get_HeadBone_Yaw());

        }

           
        //Debug.Log("MoveDir: " + DEBUG_RotateBodyMovement.Get_moveDir() + " | targetYaw: " + DEBUG_RotateBodyMovement.Get_targetYaw());
        //Debug.Log("isWalking = " +  DEBUG_relativeMovement.Get_statusAnimator()); //+ " | magnitude = " + inputMovementMagnitude);       /// DEBUG
    }



}
