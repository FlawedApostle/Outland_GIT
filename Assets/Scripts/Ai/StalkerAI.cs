using UnityEngine;
using UnityEngine.AI;
// This is is in testing phase 0.0
public class StalkerAI : MonoBehaviour
{
    public Transform player;        // User Player Position

    // NEW: simple follow mode
    public bool directFollowMode = false;
    public Transform followTarget;

    NavMeshAgent agent;
    float stalkTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.Warp(transform.position); // Fixes NavMesh errors on start
    }

    void Update()
    {
        // NEW: direct Follow Mode
        if (directFollowMode && followTarget != null)
        {
            agent.SetDestination(followTarget.position);
            return;
        }
        stalkTimer += Time.deltaTime;

        // Every 5 seconds, move to a "hiding spot" near the player
        // need to add - a navmesh radius/space checker ? (Random.insideUnitSphere * 10f); - float value 10 should be the navmesh radius / then we can implement 'hot-zones' - randomSpotInZone
        if (stalkTimer > 5f)
        {
            Vector3 randomSpotNearPlayer = player.position + (Random.insideUnitSphere * 10f);
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomSpotNearPlayer, out hit, 10f, NavMesh.AllAreas))
            {
                agent.SetDestination(hit.position);
            }
            stalkTimer = 0;
        }
    }
}