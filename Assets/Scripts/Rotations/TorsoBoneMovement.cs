using UnityEngine;

/* NOTE:
 * THIS SCRIPT IS BUILT BY SAMUEL FEARNLEY (Root00)
 * This controls the torso rotation relative to the player's movement direction.
 * The torso will rotate TOWARD the direction of movement, but only within a clamp limit.
 * This prevents unrealistic "crab walking" and adds realism to the character controller.
 */

public class TorsoBoneMovement : MonoBehaviour
{
    [Header("Torso Bone Reference")]
    [SerializeField, Tooltip("Place the torso/spine bone here")]
    private Transform torsoBone;

    [Header("Movement Input Reference")]
    [SerializeField, Tooltip("Reference to the RelativeMovement script for movement direction")]
    private RelativeMovement relativeMovement;

    [Header("Torso Rotation Settings")]
    [SerializeField, Tooltip("Maximum degrees the torso can twist left/right")]
    private float torsoYawLimit = 45f;

    [SerializeField, Tooltip("Smoothing factor for torso rotation")]
    private float rotationSmooth = 10f;

    // Internal variables
    private float clampedYaw = 0f;
    private float bodyYaw = 0f;
    private float targetYaw = 0f;
    public float Get_targetYaw()
    {
        return targetYaw;
    }

    private float moveDir;
    public float Get_moveDir()
    {
        return moveDir;
    }

    void LateUpdate()
    {

        /* 1. Get the movement direction from RelativeMovement
         * This is the SAME vector used to move the character controller.
         * It is already camera-relative, so we can use it directly.
         */
        Vector3 moveDir = relativeMovement.GetMoveDirection();
        //Debug.Log("MoveDir: " + moveDir + " | targetYaw: " + targetYaw);


        // If no movement input, smoothly return torso to neutral
        if (moveDir.sqrMagnitude < 0.01f)
        {
            clampedYaw = Mathf.Lerp(clampedYaw, 0f, rotationSmooth * Time.deltaTime);
            torsoBone.localRotation = Quaternion.Euler(0f, clampedYaw, 0f);
            return;
        }

        /* 2. Convert movement direction into a YAW angle
         * Atan2 gives us the angle in degrees relative to world forward.
         */
        targetYaw = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg;

        /* 3. Get the character body's current yaw
         * This is the root transform's Y rotation.
         */
        bodyYaw = transform.eulerAngles.y;

        /* 4. Compute the DELTA angle between body and movement direction
         * DeltaAngle converts wrapped Euler angles (0–360) into signed angles (-180 to 180)
         */
        float deltaYaw = Mathf.DeltaAngle(bodyYaw, targetYaw);

        /* 5. Clamp the torso twist so it doesn't rotate unnaturally
         * Example: -45° to +45°
         */
        clampedYaw = Mathf.Clamp(deltaYaw, -torsoYawLimit, torsoYawLimit);

        /* 6. Apply rotation to the torso bone
         * Only rotate around Y (twist). X and Z remain controlled by animations.
         */
        Quaternion targetRotation = Quaternion.Euler(0f, clampedYaw, 0f);

        // Smooth rotation for realism
        torsoBone.localRotation = Quaternion.Slerp(
            torsoBone.localRotation,
            targetRotation,
            rotationSmooth * Time.deltaTime
        );
    }
}
