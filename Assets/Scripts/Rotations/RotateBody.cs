using UnityEngine;

// THis script EXPECTS THE PLAYER TO HAVE A CHARACTER CONTROLLER

public class RotateBody : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform body;
    [SerializeField] CharacterController controller;
    [SerializeField] private Transform lookCamera; // Head / camera
        // calling movement script relative to if the player is moving
    [SerializeField] public Movement movement;

    [Header("Settings")]
    [SerializeField] float rotationSpeed = 180f;
    [SerializeField] float followDelay = 0.4f;
    [SerializeField] float minAngleToTurn = 5f;
    [SerializeField] float movementThreshold = 0.1f;
    float delayTimer = 0f;


    // MouseLook
    MouseLook mouseLook;
    // Head Direction - Snapshot
    Quaternion headDirectionSnapshot;

    private void Start()
    {
        mouseLook = GetComponent<MouseLook>();
        movement = GetComponent<Movement>();


    }

    void LateUpdate()
    {



        // 1. Observe movement
        bool isMoving = controller.velocity.magnitude > movementThreshold;
        //bool isMoving = movement.GetInput().magnitude > movementThreshold;

        // 2. Take head's world yaw (XZ plane)
        float targetYaw = lookCamera.eulerAngles.y;
        Quaternion targetRotation = Quaternion.Euler(0f, targetYaw, 0f);

        // 3. Measure yaw difference
        float angleDelta = Mathf.DeltaAngle(body.eulerAngles.y, targetYaw);

        // 4. MOVING → follow immediately
        if (isMoving)
        {
            delayTimer = 0f;

            // Rotate body towards head without affecting head
            RotateBodyWithoutAffectingHead(targetRotation);

            return;
        }

        // 5. NOT moving → delayed follow
        if (Mathf.Abs(angleDelta) > minAngleToTurn)
        {
            delayTimer += Time.deltaTime;

            if (delayTimer >= followDelay)
            {
                RotateBodyWithoutAffectingHead(targetRotation);
            }
        }
        else
        {
            // Head returned close enough → reset
            delayTimer = 0f;
            Quaternion headRot = lookCamera.rotation;
        }
    }

    void RotateBodyWithoutAffectingHead(Quaternion targetRotation)
    {
        // 1. Save head rotation
        Quaternion headRot = lookCamera.rotation;
        //Quaternion headRot = Quaternion.Euler(0f, lookCamera.eulerAngles.y, 0f);


        // 2. Rotate body
        body.rotation = Quaternion.RotateTowards(
            body.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );

        // 3. Restore head rotation
        //lookCamera.rotation = headRot;
        lookCamera.rotation = headRot;

        //// Only rotate torso yaw — do NOT touch lookCamera
        //float targetYaw = targetRotation.eulerAngles.y;
        //body.rotation = Quaternion.RotateTowards(
        //    body.rotation,
        //    Quaternion.Euler(0f, targetYaw, 0f),
        //    rotationSpeed * Time.deltaTime);

    }
}
