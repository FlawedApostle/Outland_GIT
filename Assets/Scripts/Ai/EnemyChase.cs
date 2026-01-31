using UnityEngine;
using UnityEngine.AI;

public class EnemyChase : MonoBehaviour
{
    public Transform player; // Drag the Player here in the Inspector
    private NavMeshAgent agent;
    private Animator anim;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (player != null && agent.isOnNavMesh)
        {
            // 1. Keep the destination updated to the player's CURRENT position
            agent.SetDestination(player.position);

            // 2. Only check for movement if the path is actually ready
            if (!agent.pathPending)
            {
                bool isMoving = agent.velocity.magnitude > 0.1f;
                anim.SetBool("isMoving", isMoving);
            }
        }
    }
}

// make enemy patrol
// if see player chase player for 5 seconds
// if player breaks line of sight
// go back to patroling