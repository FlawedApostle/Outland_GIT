using UnityEngine;

public class HeadBoneMovement : MonoBehaviour
{


    [SerializeField] Transform headBone;
    [SerializeField] Transform cameraTransform;
    [SerializeField] private float _cameraSmooth = 10;

    public Quaternion RotateHeadBone()
    {
        return headBone.localRotation = cameraTransform.rotation;
    }
    public Quaternion RotateHeadBone_Slerp()
    {
        Quaternion headRotationSlerp = cameraTransform.rotation;
        return headBone.rotation = Quaternion.Slerp(headBone.rotation, headRotationSlerp, _cameraSmooth * Time.deltaTime);
    }



    void LateUpdate()
    {
        // Make the head bone match the camera's rotation
        RotateHeadBone();


    }

}

/* THIS IS MY OLD HEADBONE SCRIPT - I WILL REMOVE IT EVENTUALLY
    //[SerializeField] Transform headBone; // Drag the head bone here in the Inspector
    //[SerializeField, Tooltip("Place FPS cam here")] Transform MainCameraChild; // Place the FPS camera in here
    //[SerializeField] float sensitivity = 2.0f;
    //[SerializeField] float verticalLimit = 80;

    //private float _rotationX = 0f;
    //private float _rotationY = 0f;

    ////[SerializeField] RelativeMovement _relativeMovementDir; 
    ////[SerializeField] RotateBodyMovement _rotateBodyMovement;
    //[SerializeField] Transform bodyRoot;


    //void LateUpdate() // Use LateUpdate to override standard animations
    //{
    //    // get cam absolute rotation
    //    Vector3 camChildAbsoluteRotation = MainCameraChild.eulerAngles;

    //    // Camera coords - world Space
    //    float camYaw = MainCameraChild.eulerAngles.y;
    //    float camPitch = MainCameraChild.eulerAngles.x;

    //    // body coords Yaw in world space converted to local space
    //    float bodyYaw = bodyRoot.eulerAngles.y; 
    //    bodyYaw = Mathf.DeltaAngle(0, bodyYaw);

    //    float relativeYaw = Mathf.DeltaAngle(bodyYaw, camYaw);

    //    // Clamp pitch
    //    //camPitch = Mathf.Clamp(camPitch, -verticalLimit, verticalLimit);

    //    // 2. Clamp looking up/down so the head doesn't snap backward
    //    //_rotationY = Mathf.Clamp(pitch, -verticalLimit, verticalLimit);

    //    // 3. Apply to bone
    //    // Note: Bones often have different "forward" directions. 
    //    // You may need to swap X, Y, or Z depending on your specific model rig.
    //    headBone.localRotation = Quaternion.Euler(camPitch, relativeYaw, 0f);
    // }
*/
