using UnityEngine;

public class TorsoMovement : MonoBehaviour
{
    [SerializeField] Transform torsoBone; // Drag the head bone here in the Inspector
    [SerializeField] float sensitivity = 2.0f;
    [SerializeField] float verticalLimit = 60f;

    private float _rotationX = 0f;
    private float _rotationY = 0f;

    float cameraYaw = 0f;
    float torsoYaw = 0f;

    private void LateUpdate()
    {
        //1. we need the Camera yaw !
        /// We are taking the Main Scene Camera - which has a tag NAMED "MAIN CAMERA" (WHICH IS UNITYS DEFAULT CAMERA TAG NAME) 
        cameraYaw = Camera.main.transform.eulerAngles.y;
        Debug.Log("cameraYaw eulerAngle.y : " +  cameraYaw);
        //2. We need the torso yaw
        torsoYaw = torsoBone.eulerAngles.y;
        Debug.Log("torsoYaw eulerAngle.y : " + torsoYaw);


    }

}
