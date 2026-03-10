using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;
// This is is in testing phase 0.0
public class StalkerAI : MonoBehaviour
{
    private Animator animatorPlayer;
    public Transform player;        // User Player Position
    public Transform enemy;         // Enemy Position

    // NEW: simple follow mode
    public Transform followTarget;
    public float stalk_value = 3f;
    float stalkTimer;
    float navMeshWorldRadius;
    [Tooltip ("recommended that you specify a maxDistance of twice the agent height")] public float navMesh_radius = 5f;       /// recommended that you specify a maxDistance of twice the agent height
    
    public bool directFollowMode = false;
    bool initialized = false;

    NavMeshAgent agent;
    Vector3 navMeshCenter;
    //NavMeshHit hit;



    void Start()
    {
        agent = GetComponent<NavMeshAgent>();                           // Get component player
        agent.areaMask = NavMesh.AllAreas;                              // Garuntees it walks on ANY baked area - also agent will refuse to path through EXLCUDES
        animatorPlayer = GetComponent<Animator>();                      // Get component Animator
        agent.Warp(transform.position);                                 // Fixes NavMesh errors on start
        ReadNavMeshBounds();                                            // Get NavMesh Radius/Bounds
        
    }

    void Update()
    {
        if (!initialized)
        {
            if (agent.isOnNavMesh)
                initialized = true;
            
            else
                return;
        }
        Debug.Log("isOnNavMesh = " + agent.isOnNavMesh);





        ///if (!agent.isOnNavMesh || !agent.isActiveAndEnabled) return;             //error check

        Debug.Log("Load - MODE_DIRECT_FOLLOW check");
        if (directFollowMode)
        {
            agent.SetDestination(player.position);
            //return;
        }

        else

        Debug.Log("Load - MODE_PLAYER_CHECK");
        if (player == null) return;
        Debug.Log("Load - MODE_ENEMY_CHECK");
        if (enemy == null) return;


        Debug.Log("Load - MODE_ANIMATION_LOGIC");
        // --- ANIMATION LOGIC ---
        bool isPhysicallyMoving = agent.velocity.sqrMagnitude > 0.01f;          // 0.1f is a small "buffer" so he doesn't jitter.
        animatorPlayer.SetBool("isMoving", isPhysicallyMoving);


        Debug.Log("Load - MODE_STALK_LOGIC");
        stalkTimer += Time.deltaTime;
        // Every 5 seconds, move to a "hiding spot" near the player
        // need to add - a navmesh radius/space checker ? (Random.insideUnitSphere * 10f); - float value 10 should be the navmesh radius / then we can implement 'hot-zones' - randomSpotInZone
        Vector3 dir = (player.position - transform.position).normalized;
        Vector3 target = player.position - dir * stalk_value;
        
        



        if (stalkTimer > 5f)
        {
            //NavMeshHit hit = default;
            //Position_Stalk(target);
            //GetValidPointNearPlayer();
            //Position_Stalk_Path();

            Vector3 stalkPoint = GetValidPointNearPlayer(stalk_value);
            agent.SetDestination(stalkPoint);
            stalkTimer = 0;
        }


        PrintTools.Print("DIRECTION::" , dir, "green");


        Debug.Log("Load -  PLAYER POSITION");
        // check what point ?
        IsPointInsideNavMeshBounds(player.position);                //  is the player position inside the Navmesh
        Debug.Log("Load -  ENEMY POSITION");
        IsPointInsideNavMeshBounds(enemy.position);                 //  is the enemy  position inside the Navmesh


        // DEBUG 
        NavMeshPathTest();                  // CHECK SUMMARY BELOW FUNCTION NavMeshPathTest - LEFT OFF HERE - PARTIAL PATH MEANS THAT WE MUST GO OVER THE MODULARITY OF THE WORLD - TEST EVERYTHING ! 
        DebugSampleEndpoints();

    }

    // <summary>
    //  LEFT OFF HERE - PARTIAL PATH MEANS THAT WE MUST GO OVER THE MODULARITY OF THE WORLD - TEST EVERYTHING ! 
    // </summary>
    void NavMeshPathTest()
    {
        NavMeshPath debugPath = new NavMeshPath();
        if (NavMesh.CalculatePath(transform.position, player.position, NavMesh.AllAreas, debugPath))
        {
            Debug.Log("PATH STATUS = " + debugPath.status);
        }
        else
        {
            Debug.Log("CalculatePath FAILED");
        }
    }

    void DebugSampleEndpoints()
    {
        NavMeshHit hitStart;
        NavMeshHit hitEnd;

        bool startOK = NavMesh.SamplePosition(transform.position, out hitStart, 0.5f, NavMesh.AllAreas);
        bool endOK = NavMesh.SamplePosition(player.position, out hitEnd, 0.5f, NavMesh.AllAreas);

        Debug.Log($"Sample START: {startOK} at {hitStart.position}");
        Debug.Log($"Sample END:   {endOK} at {hitEnd.position}");
    }




    /// <summary>
    ///  Function 0.
    ///  POSITION STALK
    /// </summary>
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


    /// <summary>
    ///  Function 1.
    ///  POSITION STALK PATH
    /// </summary>
    void Position_Stalk_Path()
    {
        PrintTools.Print("lOADING STALK_PATH");
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

    /// <summary>
    /// Function 2.
    ///  STALK NEAR PLAYER 
    ///  GET A POS NEAR THE PLAYER USING RADIUS SETTING FROM INSPECTOR
    /// </summary>
    Vector3 GetValidPointNearPlayer(float radius)
    {
        // Try up to 20 random points around the player
        for (int i = 0; i < 20; i++)
        {
            Vector2 circle = Random.insideUnitCircle * radius;
            Vector3 randomPoint = player.position + new Vector3(circle.x, 0, circle.y);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.5f, NavMesh.AllAreas))
            {
                return hit.position;   // Valid NavMesh point near player
            }
        }

        // Fallback: go directly to player
        return player.position;
    }


    /// <summary>
    /// NAV MESH LOGIC
    /// GIZMO - [LOOK AT QUESTION NOTES - FOR ME]
    /// GHECK IF POINT IS IN NAVMESH
    /// FIND THE NAVMESH BOUNDS - INFORMATION
    /// </summary>
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
    // GIZMO - {so this is an odd function i need to check if I can call this, as at times I cannot,
    // sounds dumb but is this a function that just runs once inside the script ?
    // i know i can place scripts inside unity and it justruns so im wondering something similar ? dumb i know}
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
    // Check for point inside the NavMesh
    bool IsPointInsideNavMeshBounds(Vector3 point)
    {
        float distance = Vector3.Distance(point, navMeshCenter);
        PrintTools.Print("Distance is inside = True" , "red");                                        // Debug
        return distance <= navMeshWorldRadius;
    }

}   // END



// OLD CODE
///Vector3 randomSpotNearPlayer = player.position + (Random.insideUnitSphere * 10f);
///if (NavMesh.SamplePosition(randomSpotNearPlayer, out hit, 10f, NavMesh.AllAreas))
///{
///    agent.SetDestination(hit.position);
///}