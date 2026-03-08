using UnityEngine;
using UnityEngine.AI;       // unity AI NavMesh

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

        public bool negativeDirection = false;

        [Header("Slide Reference")]
        public Transform slideReference;

        [Header("NavMesh")]
        public NavMeshObstacle obstacle;

        [Header("Collider (optional but recommended)")]
        public Collider doorCollider;

        [HideInInspector] public Vector3 startLocalPos;
        [HideInInspector] public Vector3 targetLocalPos;

        [HideInInspector] public bool isOpen;
    }

    [Header("Door Slots")]
    public DoorSlot doorA;
    public DoorSlot doorB;

    [Header("Detection")]
    public Transform player;
    public Transform enemy;                                             // enemy AI - for openeing the doors (for now)
    public float openDistance = 3.0f; 
    float dist = Mathf.Infinity;

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
        
        // Player open the door
        if (player != null)
        {
            float playerDistance = Vector3.Distance(distanceReference.position, player.position);
            dist  = Mathf.Min(dist, playerDistance);
        }
        // Enemy open the door
        if (enemy != null)
        {
            float enemyDist = Vector3.Distance(distanceReference.position, enemy.position);
            dist = Mathf.Min(dist, enemyDist);
        }

        //float dist = Vector3.Distance(transform.position, player.position);
        //float dist = Vector3.Distance(distanceReference.position, player.position);

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
            if (!slot.isOpen)
            {
                slot.isOpen = true;

                if (slot.obstacle != null)
                    slot.obstacle.carving = false;

                if (slot.doorCollider != null)
                    slot.doorCollider.enabled = false;
            }

            Vector3 dir = GetLocalAxis(slot.door, slot.axis);

            if (slot.negativeDirection)
                dir = -dir;

            slot.targetLocalPos = slot.startLocalPos + dir * slideDistance;
        }
        else
        {
            if (slot.isOpen)
            {
                slot.isOpen = false;

                if (slot.obstacle != null)
                    slot.obstacle.carving = true;

                if (slot.doorCollider != null)
                    slot.doorCollider.enabled = true;
            }

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