using UnityEngine;
using UnityEngine.AI;

public class EnemyPatrol : MonoBehaviour
{
    public float patrolRadius = 15f;
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
        // Pick a point within the radius
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * patrolRadius;

        NavMeshHit hit;
        // This 'snaps' the random point to the nearest valid spot on the blue NavMesh
        if (NavMesh.SamplePosition(randomPoint, out hit, patrolRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }
}