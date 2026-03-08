using UnityEngine;
using UnityEngine.AI;
// This is is in testing phase 0.0
public class StalkerAI : MonoBehaviour
{
    public Transform player;
    NavMeshAgent agent;
    float stalkTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.Warp(transform.position); // Fixes NavMesh errors on start
    }

    void Update()
    {
        stalkTimer += Time.deltaTime;

        // Every 5 seconds, move to a "hiding spot" near the player
        if (stalkTimer > 5f)
        {
            Vector3 randomSpotNearPlayer = player.position + (Random.insideUnitSphere * 10f);
            NavMeshHit hit;
            if (NavMesh.SamplePosition(player.position, out hit, 10f, NavMesh.AllAreas))   // randomSpotNearPlayer
            {
                agent.SetDestination(hit.position);
            }
            stalkTimer = 0;
        }
    }
}