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
        // 1. We use the big patrolRadius (50) to pick a spot far away
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * patrolRadius;

        NavMeshHit hit;

        // 2. We use a SMALL number (5.0f) here just to find the floor.
        // If we use patrolRadius here, the AI gets "analysis paralysis" and stands still.
        if (NavMesh.SamplePosition(randomPoint, out hit, patrolRadius, NavMesh.AllAreas)) // change to 2.0f as it needs to be 'kissing' the ground. atm 50.0f wtf is that - so im re doing the floors cause i fkd up on the mpty parents !! omfg they need to best to 0,0,0 and because i didnt do that all the floors got fkd up !! so now im re-doing it in unity once done itll be replaced to its original and PROPER VALUE. yes i swore... cause this is an annoying avoidable mistake !!
        {
            Debug.DrawLine(transform.position, hit.position, Color.red, 5f);
            agent.SetDestination(hit.position);
        }
        else
        {
            // If it fails to find a point, this tells the AI: "Try again immediately"
            timer = waitTime;
        }
    }
}