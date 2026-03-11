using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;
// This is is in testing phase 0.0
public class StalkerAI : MonoBehaviour
{
    private Animator animatorPlayer;
    public Transform player;        // User Player Position

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
    NavMeshPath path;
    //NavMeshHit hit;



    void Start()
    {
        agent = GetComponent<NavMeshAgent>();                           // Get component player - The player that HAS THE NavMesh agent
        agent.areaMask = NavMesh.AllAreas;                              // Garuntees it walks on ANY baked area - also agent will refuse to path through EXLCUDES
        animatorPlayer = GetComponent<Animator>();                      // Get component Animator
        agent.Warp(transform.position);                                 // Fixes NavMesh errors on start
        path = new NavMeshPath();
    }

    void Update()
    {
        //agent.autoRepath = true;
        /// --- INITIALIZE NAVMESH AGENT CHECK ---
        if (!initialized)
        {
            if (agent.isOnNavMesh)
                initialized = true;
            
            else
                return;
        }
        Debug.Log("isOnNavMesh = " + agent.isOnNavMesh);
        ///if (!agent.isOnNavMesh || !agent.isActiveAndEnabled) return;             //error check

        // --- ANIMATION LOGIC ---
        //Debug.Log("Load - MODE_ANIMATION_LOGIC");
        bool isPhysicallyMoving = agent.velocity.sqrMagnitude > 0.01f;          // 0.1f is a small "buffer" so he doesn't jitter.
        animatorPlayer.SetBool("isMoving", isPhysicallyMoving);

        // --- DIRECT FOLLOW TOGGLE ---
        //Debug.Log("Load - MODE_DIRECT_FOLLOW check");
        if (directFollowMode)
        {
            agent.SetDestination(player.position);
            return;
        }
        else
        { 
        //Debug.Log("Load - MODE_PLAYER_CHECK");
        if (player == null) return;
        }



        // --- STALKER LOGIC ---
        ///Debug.Log("Load - MODE_STALK_LOGIC");
        stalkTimer += Time.deltaTime;
        // Every 5 seconds, move to a "hiding spot" near the player - I'm aware were not using them. - TEsting with StalkPoint below
        /// need to add - a navmesh radius/space checker ? (Random.insideUnitSphere * 10f); - float value 10 should be the navmesh radius / then we can implement 'hot-zones' - randomSpotInZone
        Vector3 dir = (player.position - transform.position).normalized;
        Vector3 target = player.position - dir * stalk_value;

        if (stalkTimer > 5.0f)
        {
            //NavMeshHit hit = default;
            //Position_Stalk(target);
            //GetValidPointNearPlayer();
            //Position_Stalk_Path();

            Vector3 stalkPoint = GetValidPointNearPlayer(navMesh_radius);       // stalk_value
            
           
            if (agent.CalculatePath(stalkPoint, path))
            {
                Debug.Log("PATH STATUS: " + path.status);

                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    agent.SetDestination(stalkPoint);
                }

                else
                {
                    Debug.LogWarning("Target unreachable, path status: " + path.status);
                }
            }
  
            stalkTimer = 0;
        }


        //PrintTools.Print("DIRECTION::" , dir, "green");
        //Debug.Log("Load -  PLAYER POSITION");
        // check what point ?
        //IsPointInsideNavMeshBounds(player.position);                //  is the player position inside the Navmesh
        //Debug.Log("Load -  ENEMY POSITION");
        //IsPointInsideNavMeshBounds(enemy.position);                 //  is the enemy  position inside the Navmesh


        // DEBUG 
        //NavMeshPathTest();                  // CHECK SUMMARY BELOW FUNCTION NavMeshPathTest - LEFT OFF HERE - PARTIAL PATH MEANS THAT WE MUST GO OVER THE MODULARITY OF THE WORLD - TEST EVERYTHING ! 
        //DebugSampleEndpoints();

    }

    // Stalk Functions
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
    /// Function 1.
    ///  GET A POS NEAR THE PLAYER USING RADIUS SETTING FROM INSPECTOR
    /// </summary>
    Vector3 GetValidPointNearPlayer(float radius)
    {
        // Try up to 20 random points around the player
        for (int i = 0; i < 20; i++)
        {
            //Vector2 circle = Random.insideUnitCircle * radius;
            Vector2 circle = Random.insideUnitCircle.normalized * Random.Range(radius * 0.5f, radius);
            Vector3 randomPoint = player.position + new Vector3(circle.x, 0, circle.y);

            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 3.0f, NavMesh.AllAreas))
            {
                return hit.position;   // Valid NavMesh point near player
            }
        }

        // Fallback: go directly to player
        return player.position;
    }


    /// <summary>
    ///  Function 2.
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


}   // END



// OLD CODE
///Vector3 randomSpotNearPlayer = player.position + (Random.insideUnitSphere * 10f);
///if (NavMesh.SamplePosition(randomSpotNearPlayer, out hit, 10f, NavMesh.AllAreas))
///{
///    agent.SetDestination(hit.position);
///}