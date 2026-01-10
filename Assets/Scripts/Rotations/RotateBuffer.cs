using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class RotateBuffer : MonoBehaviour
{
    [Header("Script-Relative Movement")][SerializeField] private RelativeMovement relativeMovement;
    [Header("Script-Mouse Camera")][SerializeField] private MouseCamera mouseCamera;
    [SerializeField] private Transform Bone_Torso;
    [SerializeField] private Transform Bone_Head;
    [SerializeField] private Transform MainCamera;
    [SerializeField] private Animator Animator;
    [SerializeField] private float rotationSmooth = 10f;

    // Camera
    Vector3 MainCamera_Forward;
    Vector3 MainCamera_Head;

    Quaternion targetRotation_Torso;
    Quaternion targetRotation_Head;
    Quaternion HeadRotation;        // switch head
    Quaternion TorsoRotation;       // switch torso

    // Rotate Switch
    public bool Switch_Head = true;
    public bool Switch_Torso = false;
 

    void LateUpdate()
    {
        // 1. DIRECTION: Get Camera direction (Flattened to ignore pitch)
        MainCamera_Forward = MainCamera.forward;             // changed from transform MainCamera - to Script inhereitance from mouseCamera
        MainCamera_Forward.y = 0;
        MainCamera_Forward.Normalize();



        // 2. BODY ROTATION: Force body to face the camera direction ALWAYS
        // This solves the flipping problem because the body never looks at moveDir
        targetRotation_Torso = Quaternion.LookRotation(MainCamera_Forward);                     // can i use my own Quaterion
        //targetRotation_Head = MainCamera.rotation;                                            // can i use my own Quaterion
        //targetRotation_Head = Quaternion.Inverse(Bone_Torso.rotation) * MainCamera.rotation;
        targetRotation_Head =   MainCamera.rotation;

       PrintTools.Print(targetRotation_Head , "red" , "Target Rotation");



        //

        if (Switch_Head) Rotation_Head();
        if (Switch_Torso) Rotation_Torso();



        // 3. ANIMATION: Calculate relative movement for the Animator - RelativeMovement Script
        Vector3 moveDir = relativeMovement.GetMoveDirection();

        // The Dot Product tells us if we are moving Forward (1) or Backward (-1) relative to Cam
        float forwardBack = Vector3.Dot(MainCamera_Forward, moveDir);
        // The Cross Product tells us if we are moving Right (1) or Left (-1) relative to Cam
        float leftRight = Vector3.Cross(MainCamera_Forward, moveDir).y;

        // 4. FEED THE ANIMATOR: 
        // Create two Floats in your Animator Controller: "Vertical" and "Horizontal"
        //Animator.SetFloat("Vertical", forwardBack, 0.1f, Time.deltaTime);
        //Animator.SetFloat("Horizontal", leftRight, 0.1f, Time.deltaTime);
    }





public void Rotation_Head()
{
    if (!Switch_Head) return;

    // Convert camera rotation into local space relative to torso
    Quaternion localHeadTarget = Quaternion.Inverse(Bone_Torso.rotation) * targetRotation_Head;

    Bone_Head.localRotation = Quaternion.Slerp(targetRotation_Head, localHeadTarget, rotationSmooth * Time.deltaTime );
}


    public void Rotation_Torso()
    {
        if (!Switch_Torso) return;

        Bone_Torso.rotation = Quaternion.Slerp( Bone_Torso.rotation, targetRotation_Torso, rotationSmooth * Time.deltaTime );
    }


    /// Convert World To local Space
    ///   Quaternion localHeadTarget = Quaternion.Inverse(parentWorld.rotation) * childWorld.rotation;






}           /// endof


