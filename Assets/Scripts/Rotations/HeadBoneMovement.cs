using System;
using UnityEngine;

public class HeadBoneMovement : MonoBehaviour
{
    [SerializeField] Transform Transform_Bone_Head;     /// input the models bone from the hierchy
    [SerializeField] MouseCamera MouseCamera;          /// input the mouse cam script 
    [SerializeField] Transform BodyRoot;

    [SerializeField] private float _cameraSmooth = 10;

    
    public float Get_BoneHead_Yaw() {  return Transform_Bone_Head.eulerAngles.y; }       // returns the headbone, essentialy the camera yaw
    public Quaternion RotateHeadBone_Slerp() {
        Quaternion headRotationSlerp = MouseCamera.Get_MouseXYQuat();
        return Transform_Bone_Head.rotation = Quaternion.Slerp(Transform_Bone_Head.rotation, headRotationSlerp, _cameraSmooth * Time.deltaTime);
    }
    
    public Quaternion HeadBone_WorldSpace()
    {
        float headWorldYaw = Transform_Bone_Head.rotation.eulerAngles.y;
        float parentYaw = BodyRoot.parent.rotation.eulerAngles.y;
        float correctedYaw = headWorldYaw - parentYaw;
        return BodyRoot.localRotation = Quaternion.Euler(0, correctedYaw, 0);
    }

    public Quaternion HeadBone_LocalSpace()
    {
        // 1. Get camera pitch (X rotation)
        float pitch = MouseCamera.Get_MouseCamera().eulerAngles.x;
        // 2. Get camera yaw (Y rotation)
        float CamYaw = MouseCamera.Get_MouseCamera().eulerAngles.y;
        // 3. Apply BOTH to the head bone
       return Transform_Bone_Head.rotation = Quaternion.Euler(pitch, CamYaw, 0f);
        //float yaw = MouseCamera.Get_MouseXYQuat().eulerAngles.y;
        //return Transform_Bone_Head.localRotation = Quaternion.Euler(0, yaw, 0);
    }


    void LateUpdate()
    {
        // Make the head bone match the camera's rotation
        HeadBone_LocalSpace();
    }

}