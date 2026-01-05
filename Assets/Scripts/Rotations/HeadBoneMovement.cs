using UnityEngine;

public class HeadBoneMovement : MonoBehaviour
{
    [SerializeField] Transform headBone; // Drag the head bone here in the Inspector
    [SerializeField, Tooltip("Place FPS cam here")] Transform MainCameraChild; // Place the FPS camera in here
    [SerializeField] float sensitivity = 2.0f;
    [SerializeField] float verticalLimit = 60f;

    private float _rotationX = 0f;
    private float _rotationY = 0f;

    [SerializeField] RelativeMovement relativeMovementDir;


    void LateUpdate() // Use LateUpdate to override standard animations
    {
        // 1. Get Mouse Input
        //_rotationX += Input.GetAxis("Mouse X") * sensitivity;
        //_rotationY -= Input.GetAxis("Mouse Y") * sensitivity;

        // get cam absolute rotation
        Vector3 camChildAbsoluteRotation = MainCameraChild.eulerAngles;

        // convert angles to signed values
        float yaw = Mathf.DeltaAngle(0, camChildAbsoluteRotation.y);
        float pitch = Mathf.DeltaAngle(0, camChildAbsoluteRotation.x);


        //_rotationX += MainCameraChild.eulerAngles.x * sensitivity;
        //_rotationY -= MainCameraChild.eulerAngles.y * sensitivity;

        // 2. Clamp looking up/down so the head doesn't snap backward
        _rotationY = Mathf.Clamp(pitch, -verticalLimit, verticalLimit);

        // 3. Apply to bone
        // Note: Bones often have different "forward" directions. 
        // You may need to swap X, Y, or Z depending on your specific model rig.
        headBone.localRotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
