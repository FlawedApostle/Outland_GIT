using UnityEngine;
using UnityEngine.LowLevel;

/* NOTE:
 * THIS SCRIPT IS BUILT BY SAMUEL FEARNLEY (Root00)
 * This controls the torso rotation relative to the player's movement direction.
 * The torso will rotate TOWARD the direction of movement, but only within a clamp limit.
 * This prevents unrealistic "crab walking" and adds realism to the character controller.
 */

public class RotateBodyMovement : MonoBehaviour
{
    [Header("FPS Camera")]
    [SerializeField, Tooltip("FPS camera inheritance")]
    private Transform Transform_CameraFPS;

    [Header("Model - HeadBone"), Tooltip("Put actual head bone here!")]
    [SerializeField]  Transform Transform_Bone_Head;

    [Header("Model Rotate"), Tooltip("Torso From Model")]
    [SerializeField]  Transform Transform_Bone_Body;
    
    [Header("SCRIPT: RelativeMovement")]
    [SerializeField, Tooltip("RelativeMovement Script Reference")]
    private RelativeMovement relativeMovement;


    [Header("Torso Rotation Settings")]
    [SerializeField, Tooltip("Maximum degrees the torso can twist left/right")]
    private float torsoYawLimit = 45f;
    [SerializeField, Tooltip("Smoothing factor for torso rotation")]
    private float rotationSmooth = 10f;

    float _headYaw;
    float _headPitch;
    float _bodyYaw;
    Vector3 _moveDirection;
   
    public Vector3 Get_moveDir()
    {
        return _moveDirection;
    }
    
    public Quaternion Get_Transform_CameraFPS_LocalRotation() {
        return Transform_CameraFPS.localRotation;   /// local rot
    }
    public float Get_Transform_CameraFPS_Yaw() {
        return Transform_CameraFPS.eulerAngles.y;
    }
   
    public Quaternion Get_Transform_Bone_Head_LocalRotation()
    {
        return Transform_Bone_Head.localRotation;
    }
    public float Get_Transform_Bone_Head_Yaw() {
        //return _headYaw;    
        return Transform_Bone_Head.eulerAngles.y;
    }
    public float Get_Transform_Bone_Head_Pitch() {
        //return _headPitch;    // Transform_Bone_Head.eulerAngles.x
        return Transform_Bone_Head.eulerAngles.x;
    }


    public float Get_Transform_Bone_Body_Yaw() {
        return _bodyYaw;    // Transform_Bone_Head.eulerAngles.x
    }
    public void Set_Transform_Bone_BodyYaw()
    {
        _bodyYaw = Transform_Bone_Body.eulerAngles.y;
    }
    public void Set_Transform_Bone_HeadYaw()
    {
        _headYaw = Transform_Bone_Head.eulerAngles.y;
    }
    public void Set_Transform_Transform_Bone_HeadPitch()
    {
        _headPitch = Transform_Bone_Head.eulerAngles.x;
    }



    void LateUpdate()
    {
        _headYaw = Transform_Bone_Head.eulerAngles.y;
        _headPitch = Transform_Bone_Head.eulerAngles.x;
        _bodyYaw = Transform_Bone_Body.eulerAngles.y;


        _moveDirection = relativeMovement.GetMoveDirection();




        // If no movement input, smoothly return torso to neutral
        //if (moveDir.sqrMagnitude < 0.01f)
        //{
        //float clampedYaw = Mathf.Lerp(0f, 0f, rotationSmooth * Time.deltaTime);
        //transformBodyRoot.rotation = Quaternion.Euler(0, MainCameraChild.eulerAngles.y, 0);       // face the came even when not moving
        //return;
        //}

    }
}




/*      OLD MOVEMENT DIRECTION SCRIPT - TORSO ROTATES TO DIRECTION OF MOVEMENT - WILL REMOVE EVENTUALLY
       
    [Header("Player Root Empty")]
    [SerializeField, Tooltip("Place Empty with all scripts")]
    private Transform Transform_Empty;
    [SerializeField] private float rotationSpeed = 5f;
    // Internal variables
    private float clampedYaw = 0f;
    private float bodyYaw = 0f;
    private float targetYaw = 0f;
    public float Get_targetYaw()
    {
        float temp = targetYaw;
        return temp;
    }
    private Vector3 moveDir;
    public Vector3 Get_moveDir()
    {
        Vector2 temp = moveDir;
        return temp;
    }

moveDir = relativeMovement.GetMoveDirection();
        //Debug.Log("MoveDir: " + moveDir + " | targetYaw: " + targetYaw);

        // If no movement input, smoothly return torso to neutral
        if (moveDir.sqrMagnitude < 0.01f)
        {
            clampedYaw = Mathf.Lerp(clampedYaw, 0f, rotationSmooth * Time.deltaTime);
            //transformBodyRoot.rotation = Quaternion.Euler(0, MainCameraChild.eulerAngles.y, 0);       // face the came even when not moving
            return;
        }

        Vector3 worldMove = Transform_Empty.TransformDirection(moveDir);      // legit line to convert from local to world space.......
        targetYaw = Mathf.Atan2(worldMove.x, worldMove.z) * Mathf.Rad2Deg;      // calling the converted coords to apply rotation correctly

        bodyYaw = Transform_Empty.eulerAngles.y;                  // empty controller that holds the player
        float cameraYaw = Transform_FPSCamera.eulerAngles.y;            // camera direction and where its facing on the y axis

        ///* 4. Compute the DELTA angle between body and movement direction
        float deltaYaw = Mathf.DeltaAngle(bodyYaw, cameraYaw);
        
        ///* 5. Clamp the torso twist so it doesn't rotate unnaturally Example: -45° to +45°
        clampedYaw = Mathf.Clamp(deltaYaw, -torsoYawLimit, torsoYawLimit);

        ///* 6. Apply rotation to the torso bone
        // * Only rotate around Y (twist). X and Z remain controlled by animations.
        Quaternion targetRotation = Quaternion.Euler(worldMove.x, cameraYaw, worldMove.z);
        //Quaternion targetRotation = Quaternion.Euler(0f, cameraYaw, 0f);

        // Smooth rotation for realism
        Transform_Empty.rotation = Quaternion.Slerp(Transform_Empty.rotation, targetRotation, rotationSmooth * Time.deltaTime);
        //transformBodyRoot.localRotation = Quaternion.Slerp(transformBodyRoot.rotation, targetRotation, rotationSmooth * Time.deltaTime);
        //transformBodyRoot.rotation = Quaternion.Euler(0, cameraYaw, 0);
 */