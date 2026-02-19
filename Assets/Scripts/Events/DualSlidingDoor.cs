using UnityEngine;
using UnityEngine.UIElements;

public class DualSlidingDoor : MonoBehaviour
{
    [Header("Object References")]
    [SerializeField] private Transform doorLeft;
    [SerializeField] private Transform doorRight;

    [Header("Movement Settings")]
    public Vector3 slideDirection_L = new Vector3(1, 0, 0); // Direction for Door A
    public Vector3 slideDirection_R = new Vector3(-1, 0, 0); // Direction for Door B
    public float slideDistance = 1.2f;
    public float speed = 3.0f;

    private Vector3 leftStartPos;
    private Vector3 rightStartPos;
    private Vector3 leftTarget;
    private Vector3 rightTarget;

    void Start()
    {
        // Save the closed positions
        leftStartPos = doorLeft.localPosition;
        rightStartPos = doorRight.localPosition;

        // Default target is closed
        leftTarget = leftStartPos;
        rightTarget = rightStartPos;
    }

    void Update()
    {
        // Move both doors smoothly every frame
        doorLeft.localPosition = Vector3.MoveTowards(doorLeft.localPosition, leftTarget, speed * Time.deltaTime);
        doorRight.localPosition = Vector3.MoveTowards(doorRight.localPosition, rightTarget, speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            leftTarget = leftStartPos + (slideDirection_L * slideDistance);
            rightTarget = rightStartPos + (slideDirection_R * slideDistance); // The '-' makes it opposite
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            leftTarget = leftStartPos;
            rightTarget = rightStartPos;
        }
    }
}