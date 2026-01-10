using UnityEngine;

public class RotateBuffer : MonoBehaviour
{
    [Header("Script-Relative Movement")][SerializeField] private RelativeMovement relativeMovement;
    [SerializeField] private Transform Bone_Torso;
    [SerializeField] private Transform Bone_Head;
    [SerializeField] private Transform mainCamera;
    [SerializeField] private Animator animator;
    [SerializeField] private float rotationSmooth = 10f;

    // Camera
    Vector3 MaincamFwd

    Quaternion targetRotation;
    Quaternion HeadRotation;        // switch head
    Quaternion TorsoRotation;       // switch torso

    // Rotate Switch
    public bool Head_Switch = true;
    public bool Torso_Switch = false;

    void LateUpdate()
    {
        // 1. DIRECTION: Get Camera direction (Flattened to ignore pitch)
        Vector3 camForward = mainCamera.forward;
        camForward.y = 0;
        camForward.Normalize();

        // 2. BODY ROTATION: Force body to face the camera direction ALWAYS
        // This solves the flipping problem because the body never looks at moveDir
        targetRotation = Quaternion.LookRotation(camForward);
        //Rotation_Head();
        Rotation_Torso();



        // 3. ANIMATION: Calculate relative movement for the Animator - RelativeMovement Script
        Vector3 moveDir = relativeMovement.GetMoveDirection();

        // The Dot Product tells us if we are moving Forward (1) or Backward (-1) relative to Cam
        float forwardBack = Vector3.Dot(camForward, moveDir);
        // The Cross Product tells us if we are moving Right (1) or Left (-1) relative to Cam
        float leftRight = Vector3.Cross(camForward, moveDir).y;

        // 4. FEED THE ANIMATOR: 
        // Create two Floats in your Animator Controller: "Vertical" and "Horizontal"
        //animator.SetFloat("Vertical", forwardBack, 0.1f, Time.deltaTime);
        //animator.SetFloat("Horizontal", leftRight, 0.1f, Time.deltaTime);
    }





    public Quaternion Rotation_Head()
    {
        if (Head_Switch != false)
        {
            return Bone_Torso.rotation = Quaternion.Slerp(Bone_Torso.rotation, targetRotation, rotationSmooth * Time.deltaTime);
        } return Bone_Torso.rotation = TorsoRotation;
    }
    public Quaternion Rotation_Torso()
    {
        if (Torso_Switch != false){
            HeadRotation = Quaternion.Slerp(Bone_Torso.rotation, targetRotation, rotationSmooth * Time.deltaTime);
        } return Bone_Torso.rotation = HeadRotation;
    }







}           /// endof


