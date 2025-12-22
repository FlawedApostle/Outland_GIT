using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.XR;
// Relative Camera Movement. Player will follow facing the direction of the camera (head)

public class RelativeMovement : MonoBehaviour
{
    //[SerializeField] public Transform Camera;
    [SerializeField] float speed = 5f;
    void Update()
    {
        // 1. Get raw camera directions without an Inspector slot
        Vector3 camForward = Camera.main.transform.forward;
        Vector3 camRight = Camera.main.transform.right;

        // 2. Flatten the vectors so you don't move into the ground
        camForward.y = 0;
        camRight.y = 0;

        // 3. Re-normalize to ensure the length is exactly 1
        camForward.Normalize();
        camRight.Normalize();

        // 4. Debug output to show current movement directions
        Debug.Log($"Cam Relative Forward: {camForward} | Cam Relative Right: {camRight}");

        // Bonus: Show the orientation as a Quaternion if needed
        Quaternion camOrientation = Camera.main.transform.rotation;
        //Debug.Log($"Full Camera Orientation (Quaternion): {camOrientation}");

        // 1. Get input from WASD or Joystick
        float horizontal = UnityEngine.Input.GetAxis("Horizontal");
        float vertical = UnityEngine.Input.GetAxis("Vertical");

        // 2. Get and "Flatten" camera directions (ignore tilt)
        camForward = Camera.main.transform.forward;
        camRight = Camera.main.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        // 3. Combine into a final movement vector
        Vector3 moveDirection = (camForward * vertical) + (camRight * horizontal);

        // 4. Apply movement (using normalization to keep speed consistent)
        if (moveDirection.magnitude > 0.1f)
        {
            // Move the object
            transform.position += moveDirection.normalized * speed * Time.deltaTime;

            // Debug the final direction vector
            ///Debug.Log($"[RelativeMovement DEBUG] Moving in relative direction: {moveDirection.normalized}");
        }
    }
}
