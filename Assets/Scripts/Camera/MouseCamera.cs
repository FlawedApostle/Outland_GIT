using Unity.VisualScripting;
using UnityEngine;

public class MouseCamera : MonoBehaviour
{
    [SerializeField] float sensitivity = 2.0f;
    [SerializeField] float verticalLookLimit = 80f;
    [SerializeField] float horizontalLookLimit = 80f;

    private float _rotationX = 0f; // Left/Right (Yaw)
    private float _rotationY = 0f; // Up/Down (Pitch)

    void Start()
    {
        // Lock the cursor to the center of the screen for better control
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    void Update()
    {

        // 1. Get the change in mouse position since last frame
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        // 2. Accumulate the values
        _rotationX += mouseX;
        _rotationY -= mouseY; // Inverted so moving mouse up looks up

        // 3. Clamp the vertical look to prevent the camera from flipping over
        _rotationY = Mathf.Clamp(_rotationY, -verticalLookLimit, verticalLookLimit);
        
        /// 3.5 Clamp the vertical look to prevent the camera from flipping over
        /// We must set if the clamp reaches its limit the clamp reverts back to zero to then continue rotation in a realistic manner
        //_rotationX = Mathf.Clamp(_rotationX, -horizontalLookLimit, horizontalLookLimit);

        // 4. Apply the rotation directly to the camera
        transform.eulerAngles = new Vector3(_rotationY, _rotationX, 0f);


        // Get the rotation as a Quaternion (best for math/interpolation)
        Quaternion camRotation = Camera.main.transform.rotation;
        // Get the rotation as Euler angles (Vector3 - X, Y, Z)
        Vector3 camEulerAngles = Camera.main.transform.eulerAngles;

        // Example: Make this object face the same way as the camera's forward direction Sets forward direction
        /// transform.forward = Camera.main.transform.forward;
        //  Or use the rotation directly Sets full rotation
        /// transform.rotation = camRotation;

        Debug.Log("Camera Rotation (Euler): " + transform.eulerAngles);
    }


}
