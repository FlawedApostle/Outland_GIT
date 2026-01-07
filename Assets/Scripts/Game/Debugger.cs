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


    private void Start()
    {
        NULLCHECK_HeadBoneMovement();
    }

    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.F1))  {
            Debug.Log("[HeadBoneMovement & RotateBodyMovement]");
            Debug.Log("[Bone Yaw] " + DEBUG_HeadBoneMovement.Get_Transform_Bone_Head_YAW());
            Debug.Log("[HeadYaw] " + DEBUG_RotateBodyMovement.Get_HeadYaw());

        }

        if (Input.GetKeyDown(KeyCode.F2)){
            Debug.Log("[MouseCamera]");
            MouseCam_Coords();
            MouseCam_Yaw();

        }
        if (Input.GetKeyDown(KeyCode.F3)){
            Debug.Log("[RotateBodyMovement]");
            Debug.Log("Bone Pitch] " + DEBUG_RotateBodyMovement.Get_HeadPitch());
            Debug.Log("[BodyYaw] " +  DEBUG_RotateBodyMovement.Get_BodyYaw());

        }


        //Debug.Log("MoveDir: " + DEBUG_RotateBodyMovement.Get_moveDir() + " | targetYaw: " + DEBUG_RotateBodyMovement.Get_targetYaw());
        //Debug.Log("isWalking = " +  DEBUG_relativeMovement.Get_statusAnimator()); //+ " | magnitude = " + inputMovementMagnitude);       /// DEBUG
    }



}
