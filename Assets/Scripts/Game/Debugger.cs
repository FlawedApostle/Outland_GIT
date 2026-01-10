using UnityEngine;
using UnityEngine.UIElements;
using System;
using Unity.VisualScripting;

public class Debuger : MonoBehaviour
{
    [Header("SCRIPTS: MouseCamera")]
    [SerializeField] private MouseCamera DEBUG_MouseCamera;

    [Header("SCRIPTS: Relative Movement")]
    [SerializeField, Tooltip("Reference to the RelativeMovement script for movement direction")]
    private RelativeMovement DEBUG_RelativeMovement;

    [Header("SCRIPTS: Rotate Body Movement")]
    [SerializeField]// Tooltip("Reference to the RelativeMovement script for movement direction")]
    private RotateBodyMovement DEBUG_RotateBodyMovement;
    
    [Header("SCRIPTS: Head Bone Movement")]
    [SerializeField, Tooltip("Reference to the RelativeMovement script for movement direction")]
    private HeadBoneMovement DEBUG_HeadBoneMovement;

    [Header("SCRIPTS: Animator Script Reference")]
    private Animator DEBUG_animator;


    public float NULLCHECK_HeadBoneMovement(){
        if (DEBUG_HeadBoneMovement == null) { Debug.LogError("DEBUG_HeadBoneMovement is NULL!"); }
        return 0f;
    }
    
    public void MouseCam_Coords() { /// raw values multiplyed by sensitivity
        Debug.Log("Mouse Cam Coords: (" 
            + DEBUG_MouseCamera.Get_MouseXYQuat().x + " , " + DEBUG_MouseCamera.Get_MouseXYQuat().y + ")");    
    }
    public void MouseCam_Yaw() { /// raw values multiplyed by sensitivity
        Debug.Log("Mouse Cam Coords Yaw: ("
            + DEBUG_MouseCamera.Get_MouseXYQuat().eulerAngles.y);
    }


    private void Awake()
    {
        //NULLCHECK_HeadBoneMovement();
    }

    private void Update()
    {
        /// ROTATE BODY && HEADBONE
        if (Input.GetKeyDown(KeyCode.F1))  {
            
            Debug.Log("[GET YAW - HeadBoneMovement & RotateBodyMovement]");
            Debug.Log("[Yaw-BONE HEAD] " + DEBUG_HeadBoneMovement.Get_BoneHead_Yaw());
            Debug.Log("[Yaw-HEAD] " + DEBUG_RotateBodyMovement.Get_HeadYaw());
            Debug.Log("[Yaw-BODY] " + DEBUG_RotateBodyMovement.Get_BodyYaw());

        }
        /// MOUSE CAM
        //_MouseCamera_Forward = _MouseCamera.Get_MouseCamera().forward;
        if (Input.GetKeyDown(KeyCode.F2)){
            Debug.Log("[Mouse Camera]");
            Debug.Log("Mouse Cam Coords: ("     + DEBUG_MouseCamera.Get_MouseXYQuat().x + " , " + DEBUG_MouseCamera.Get_MouseXYQuat().y + ")" );
            Debug.Log("Mouse Cam Coords Yaw: "  + DEBUG_MouseCamera.Get_MouseXYQuat().eulerAngles.y);

        }
        /// ROTATE BODY 
        if (Input.GetKeyDown(KeyCode.F3)){
            Debug.Log("[RotateBody Movement]");
            Debug.Log("[HeadYaw] "      + DEBUG_RotateBodyMovement.Get_HeadYaw());
            Debug.Log("[BodyYaw] "      + DEBUG_RotateBodyMovement.Get_BodyYaw());
            Debug.Log("Bone Pitch] "    + DEBUG_RotateBodyMovement.Get_HeadPitch());
            Debug.Log("[Mouse Camera Forward Transform] "      + DEBUG_RotateBodyMovement.Get_MouseCamera_Forward());

        }
        /// HEAD BONE
        if (Input.GetKeyDown(KeyCode.F4)) {
            Debug.Log("[HEAD BONE]");
            Debug.Log("[Bone Yaw] " + DEBUG_HeadBoneMovement.Get_BoneHead_Yaw());        // shows the bone yaw from the model

        }


        /*
        if (Input.GetKeyDown(KeyCode.F5))
        {
            Debug.Log("[HEAD BONE]");
            Debug.Log("[Bone Yaw] " + DEBUG_HeadBoneMovement.Get_Transform_Bone_Head_YAW());        // shows the bone yaw from the model

        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            Debug.Log("[RotateBody Movement]");
            Debug.Log("[HeadYaw] " + DEBUG_RotateBodyMovement.Get_HeadYaw());
            Debug.Log("[BodyYaw] " + DEBUG_RotateBodyMovement.Get_BodyYaw());
            Debug.Log("Bone Pitch] " + DEBUG_RotateBodyMovement.Get_HeadPitch());
            Debug.Log("[Mouse Camera Forward Transform] " + DEBUG_RotateBodyMovement.Get_MouseCamera_Forward());
        }
        if (Input.GetKeyDown(KeyCode.F7))
        {
            Debug.Log("[Mouse Camera]");
            Debug.Log("Mouse Cam Coords Yaw: " + DEBUG_MouseCamera.Get_MouseXYQuat().eulerAngles.y);
            Debug.Log("Mouse Cam Coords: (" + DEBUG_MouseCamera.Get_MouseXYQuat().x + " , " + DEBUG_MouseCamera.Get_MouseXYQuat().y + ")");
        }
        */


        //Debug.Log("MoveDir: " + DEBUG_RotateBodyMovement.Get_moveDir() + " | targetYaw: " + DEBUG_RotateBodyMovement.Get_targetYaw());
        //Debug.Log("isWalking = " +  DEBUG_relativeMovement.Get_statusAnimator()); //+ " | magnitude = " + inputMovementMagnitude);       /// DEBUG
    }



}
