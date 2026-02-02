using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    [Header ("DEBBUGGING TOOLS FOR INSPECTOR")]
    [Tooltip("How far should the random point look for. - The larger the number the greater the search radius - Using 2d insideUnitCircle to ENSURE y axis is zereod out")]
    public float patrolRadius = 15f;
    [Tooltip("Floor Distance - testing the distance from the baked nav Mesh and the random Point selection")]
    public float floorDistance = 50f;
    [Tooltip("How long will the enemy 'look' at the random point selection")]
    public float waitTime = 3f;

    private NavMeshAgent agent;
    private Animator anim;
    private float timer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        // --- MOVEMENT LOGIC ---
        // If we are close to our target, start the wait timer
        if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
        {
            timer += Time.deltaTime;
            if (timer >= waitTime)
            {
                GoToRandomPoint();
                timer = 0;
            }
        }

        // --- ANIMATION LOGIC ---
        // We check the ACTUAL speed of the model. 
        // 0.1f is a small "buffer" so he doesn't jitter.
        //bool isPhysicallyMoving = agent.velocity.magnitude > 0.1f;
        bool isPhysicallyMoving = agent.velocity.sqrMagnitude > 0.01f;
        anim.SetBool("isMoving", isPhysicallyMoving);
    }


    void GoToRandomPoint()
    {
        // 1. generate a random point in world space. 
        // Use insideUnitCircle so the 'Y' is always 0 relative to the enemy - insideUnitCirlce is 2D ! so its flat against the plane of the baked nav Mesh
        Vector2 randomCircle = Random.insideUnitCircle * patrolRadius;
        Vector3 randomPoint = new Vector3(transform.position.x + randomCircle.x, transform.position.y, transform.position.z + randomCircle.y);
        NavMeshHit hit; 
        bool randomPointTrue;
        randomPointTrue = NavMesh.SamplePosition(randomPoint, out hit, floorDistance, NavMesh.AllAreas);

        // 2. We use a SMALL number (5.0f) here just to find the floor.
        // If we use patrolRadius here, the AI gets "analysis paralysis" and stands still.
        if (randomPointTrue) // this will break navMesh Ai controll if messed with change to 2.0f  this is fixed, however I have a PUBLIC GLOBAL variable set in inspector. its mainly for debugging purposes. REMOVE IN RELEASE VERSION
        {
            Debug.DrawLine(transform.position, hit.position, Color.red, 5f);
            agent.SetDestination(hit.position);
        }
        else
        {
            // Use your 3-parameter Print: Value, Color, Title
            PrintTools.Print("False", "--- NAVMESH SEARCH FAILED ---", "red");

            // Extracting the attempted vertical position
            PrintTools.Print("Attempted Y Height", randomPoint.y, "yellow");
            // If it fails to find a point, this tells the AI: "Try again immediately"
            timer = waitTime;
        }
    }
}