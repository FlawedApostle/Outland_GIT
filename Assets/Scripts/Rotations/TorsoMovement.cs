using UnityEngine;

public class TorsoMovement : MonoBehaviour
{
    [SerializeField] Transform torsoBone; // Drag the head bone here in the Inspector
    [SerializeField] Transform headBoneYaw; // get the direction of the headBone - ONLY the yaw 
    [SerializeField] float sensitivity = 2.0f;
    [SerializeField] float verticalLimit = 60f;

    private float _rotationX = 0f;
    private float _rotationY = 0f;

    float cameraYaw = 0f;
    float torsoYaw = 0f;
    float headYaw = 0f;

    float followThreshold = 3f;
    float followSpeed = 3f;

    private void LateUpdate()
    {
        //1. we need the Camera yaw !
        /// We are taking the Main Scene Camera - which has a tag NAMED "MAIN CAMERA" (WHICH IS UNITYS DEFAULT CAMERA TAG NAME) 
        cameraYaw = Camera.main.transform.eulerAngles.y;
        Debug.Log("cameraYaw eulerAngle.y : " +  cameraYaw);
        //2. We need the torso yaw
        torsoYaw = torsoBone.eulerAngles.y;
        Debug.Log("torsoYaw eulerAngle.y : " + torsoYaw);
        // 3. we need the head yaw
        headYaw = headBoneYaw.eulerAngles.y;
        Debug.Log("headYaw eulerAngle.y : " + headYaw);
        // 4. we need relative camera 'movememt' ?
        // do i need to get relative movement ? I dont think so no.

        // if the head yaw is facing a direction for a period of time 
        // then rotate the body yaw to face the head direction
        float yawDelta = Mathf.DeltaAngle(torsoYaw, cameraYaw);
        if (Mathf.Abs(yawDelta) > followThreshold)
        {
            float targetYaw = Mathf.LerpAngle( torsoYaw, cameraYaw, followSpeed * Time.deltaTime );
            torsoBone.rotation = Quaternion.Euler(0f, targetYaw, 0f);
        }



    }

}
