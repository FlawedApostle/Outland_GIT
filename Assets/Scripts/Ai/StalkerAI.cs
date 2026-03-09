using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;
// This is is in testing phase 0.0
public class StalkerAI : MonoBehaviour
{
    public Transform player;        // User Player Position

    // NEW: simple follow mode
    public bool directFollowMode = false;
    public Transform followTarget;
    public float stalk_value = 3f;
    [Tooltip ("recommended that you specify a maxDistance of twice the agent height")] public float navMesh_radius = 5f;       /// recommended that you specify a maxDistance of twice the agent height

    Vector3 navMeshCenter;
    float navMeshWorldRadius;

    //NavMeshHit hit;

    NavMeshAgent agent;
    float stalkTimer;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();                           // Get component player
        agent.Warp(transform.position);                                 // Fixes NavMesh errors on start
        ReadNavMeshBounds();                                            // Get NavMesh Radius/Bounds
        OnDrawGizmos();                                                 // Draw NavMesh
    }

    void Update()
    {
        //error check
        if (!agent.isOnNavMesh || !agent.isActiveAndEnabled) return;
        
        // Direct Follow Mode
        if (directFollowMode && followTarget != null)
        {
            agent.SetDestination(followTarget.position);
            return;
        }
        
        stalkTimer += Time.deltaTime;
        // Every 5 seconds, move to a "hiding spot" near the player
        // need to add - a navmesh radius/space checker ? (Random.insideUnitSphere * 10f); - float value 10 should be the navmesh radius / then we can implement 'hot-zones' - randomSpotInZone
        Vector3 dir = (player.position - transform.position).normalized;
        Vector3 target = player.position - dir * stalk_value;
        if (stalkTimer > 5f)
        {
            //NavMeshHit hit = default;
            //Position_Stalk(target);
            Position_Stalk_Path();
            stalkTimer = 0;
        }
    }


    // Read The NavMesh Bounds
    void ReadNavMeshBounds()
    {
        var triangulation = NavMesh.CalculateTriangulation();
        var vertices = triangulation.vertices;

        if (vertices.Length == 0) return;

        Vector3 min = vertices[0];
        Vector3 max = vertices[0];

        foreach (var v in vertices)
        {
            min = Vector3.Min(min, v);
            max = Vector3.Max(max, v);
        }

        navMeshCenter = (min + max) * 0.5f;
        navMeshWorldRadius = Vector3.Distance(min, max) * 0.5f;
    }

    // Draw The NavMesh Bounds
    void OnDrawGizmos()
    {
        var triangulation = NavMesh.CalculateTriangulation();

        Gizmos.color = Color.violetRed;

        for (int i = 0; i < triangulation.indices.Length; i += 3)
        {
            Vector3 a = triangulation.vertices[triangulation.indices[i]];
            Vector3 b = triangulation.vertices[triangulation.indices[i + 1]];
            Vector3 c = triangulation.vertices[triangulation.indices[i + 2]];

            Gizmos.DrawLine(a, b);
            Gizmos.DrawLine(b, c);
            Gizmos.DrawLine(c, a);
        }
    }



    void Position_Stalk(Vector3 target)
    {
        /// Output parameter that Unity fills with information about the nearest NavMesh point.
        NavMeshHit hit;// = default;
       
        if (NavMesh.SamplePosition(target, out hit, navMesh_radius, NavMesh.AllAreas))
        {   // raycast
            Debug.DrawLine(transform.position, hit.position, Color.red, 1f);
            // to stop spamming the same point
            if (Vector3.Distance(agent.destination, hit.position) > 1f)         //agent.SetDestination(hit.position);
            {
                agent.SetDestination(hit.position);                             // hit.position player.position
            }
        }
    }


    void Position_Stalk_Path()
    {
        NavMeshPath path = new NavMeshPath();

        if (NavMesh.CalculatePath(transform.position, player.position, NavMesh.AllAreas, path))
        {
            if (path.status == NavMeshPathStatus.PathComplete && path.corners.Length > 1)
            {
                Vector3 nextCorner = path.corners[1];

                Debug.DrawLine(transform.position, nextCorner, Color.green, 1f);

                if (Vector3.Distance(agent.destination, nextCorner) > 1f)
                {
                    agent.SetDestination(nextCorner);
                }
            }
        }
    }


}   // END



// OLD CODE
///Vector3 randomSpotNearPlayer = player.position + (Random.insideUnitSphere * 10f);
///if (NavMesh.SamplePosition(randomSpotNearPlayer, out hit, 10f, NavMesh.AllAreas))
///{
///    agent.SetDestination(hit.position);
///}