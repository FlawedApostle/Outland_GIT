using UnityEngine;

public class ProximityDualSlidingDoor : MonoBehaviour
{
    public enum Axis
    {
        X,
        Y,
        Z
    }

    [System.Serializable]
    public class DoorSlot
    {
        [Header("Door Transform")]
        public Transform door;

        [Header("Movement Axis")]
        public Axis axis = Axis.X;

        [Tooltip("Positive = Right/Up/Forward, Negative = Left/Down/Back")]
        public bool negativeDirection = false;

        [Header("Slide Reference (for auto-direction)")]
        public Transform slideReference;

        [HideInInspector] public Vector3 startLocalPos;
        [HideInInspector] public Vector3 targetLocalPos;
    }

    [Header("Door Slots")]
    public DoorSlot doorA;
    public DoorSlot doorB;

    [Header("Detection")]
    public Transform player;
    public float openDistance = 3.0f;

    [Header("Movement Settings")]
    public float slideDistance = 1.2f;
    public float speed = 3.0f;

    [SerializeField] private Transform distanceReference;               // door center reference
    
    void Start()
    {
        InitializeDoor(doorA);
        InitializeDoor(doorB);
    }

    void Update()
    {
        if (player == null) return;

        //float dist = Vector3.Distance(transform.position, player.position);
        float dist = Vector3.Distance(distanceReference.position, player.position);

        bool shouldOpen = dist <= openDistance;

        UpdateDoorTarget(doorA, shouldOpen);
        UpdateDoorTarget(doorB, shouldOpen);

        MoveDoor(doorA);
        MoveDoor(doorB);
    }

    void InitializeDoor(DoorSlot slot)
    {
        if (slot.door == null) return;

        slot.startLocalPos = slot.door.localPosition;
        slot.targetLocalPos = slot.startLocalPos;
    }

    void UpdateDoorTarget(DoorSlot slot, bool open)
    {
        if (slot.door == null) return;

        if (open)
        {
            Vector3 dir = GetLocalAxis(slot.door, slot.axis);
            slot.targetLocalPos = slot.startLocalPos + dir * slideDistance;
            //Vector3 dir = (slot.slideReference.position - slot.door.position).normalized;

            if (slot.negativeDirection)
                dir = -dir;

            slot.targetLocalPos = slot.startLocalPos + dir * slideDistance;
        }
        else
        {
            slot.targetLocalPos = slot.startLocalPos;
        }
    }

    void MoveDoor(DoorSlot slot)
    {
        if (slot.door == null) return;

        slot.door.localPosition = Vector3.MoveTowards(
            slot.door.localPosition,
            slot.targetLocalPos,
            speed * Time.deltaTime);
    }

    Vector3 GetLocalAxis(Transform t, Axis axis)
    {
        switch (axis)
        {
            case Axis.X: return t.right;
            case Axis.Y: return t.up;
            case Axis.Z: return t.forward;
        }

        return Vector3.right;
    }
}