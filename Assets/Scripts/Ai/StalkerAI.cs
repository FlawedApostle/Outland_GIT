using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms;
using static UnityEngine.GraphicsBuffer;
// This is is in testing phase 1.1
public class StalkerAI : MonoBehaviour
{
    //public Animator animatorScript;
    [SerializeField] private Animator animatorPlayer;
    public Transform player;        // User Player Position
    public Transform followTarget;


    // NEW inspector options
    [Tooltip("Radius to search for valid nav points around player")]
    public float navMesh_radius = 5f;
    public float stalk_value = 3f;
    public bool directFollowMode = false;
    [Range (0.1f , 5.0f)] public float Enemy_Distance = 0.2f;
    public LayerMask environmentMask = ~0; // set in inspector to only world (walls/floors), default everything

    float stalkTimer;
    bool initialized = false;

    NavMeshAgent agent;
    NavMeshPath path;
    //NavMeshHit hit;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();                           /// Get component player - The player that HAS THE NavMesh agent
        agent.areaMask = NavMesh.AllAreas;                              /// Garuntee it walks on ANY baked area - also agent will refuse to path through EXLCUDES
        agent.Warp(transform.position);                                 /// Fixes NavMesh errors on start
        path = new NavMeshPath();                                       /// NavMesh Pathing Ai

        // How far the enemy stops (tweak in inspector)
        agent.stoppingDistance = 1.2f;                                  /// Tune this to player's radius + desired buffer
    }

    void Update()
    {
        /// --- INITIALIZE NAVMESH AGENT CHECK ---
        if (!initialized)
        {
            if (agent.isOnNavMesh) initialized = true;
            
            else return;
        }
        //Debug.Log("isOnNavMesh = " + agent.isOnNavMesh);

        /// --- ANIMATION LOGIC ---
        bool isPhysicallyMoving = agent.velocity.sqrMagnitude > 0.01f;          // 0.1f is a small "buffer" so he doesn't jitter.
        if(animatorPlayer) animatorPlayer.SetBool("isMoving", isPhysicallyMoving);

        /// --- DIRECT FOLLOW TOGGLE ---
        if (directFollowMode)
        {
            SetAgentDestinationWithStop(player.position); //agent.SetDestination(player.position);
            return;
        }
        if (player == null) return;
        PrintTools.Print("Error Player ::", player, "red");


        /// --- STALKER LOGIC ---
        stalkTimer += Time.deltaTime;
        // Every 5 seconds, move to a "hiding spot" near the player - I'm aware were not using them. - TEsting with StalkPoint below
        /// need to add - a navmesh radius/space checker ? (Random.insideUnitSphere * 10f); - float value 10 should be the navmesh radius / then we can implement 'hot-zones' - randomSpotInZone
        ///Vector3 dir = (player.position - transform.position).normalized;
        ///Vector3 target = player.position - dir * stalk_value;
        if (stalkTimer > 5.0f)
        {
            //Vector3 stalkPoint = GetValidPointNearPlayer(navMesh_radius);             // stalk_value
            //Vector3 stalkPoint = GetRefinedStalkPoint(navMesh_radius);                // stalk_value
            Vector3 stalkPoint = GetAValidPointNearPlayer(navMesh_radius);              // stalk_value
            // Check if path is complete first
            if (agent.CalculatePath(stalkPoint, path) && path.status == NavMeshPathStatus.PathComplete)
            {
                agent.SetDestination(stalkPoint);
            }

            else
            {
                // fallback to player if unreachable
                agent.SetDestination(player.position);    /// SetAgentDestinationWithStop(player.position);
            }
  
            stalkTimer = 0;
        }

        /// --- ENFORCE STOPPING DISTANCE
        // ALWAYS enforce stoppingDistance (prevents walking into player)
        EnforceStoppingDistance();



        //if (!agent.pathPending)
        //    Debug.Log("Agent path status: " + agent.pathStatus + ", remainingDist=" + agent.remainingDistance);
        NavMeshHit hit;
        Vector3 testPos = new Vector3(transform.position.x, transform.position.y, transform.position.z); // e.g. player position or door threshold
        bool onMesh = NavMesh.SamplePosition(testPos, out hit, 0.5f, NavMesh.AllAreas);
        Debug.Log("SamplePosition(" + testPos + "): " + onMesh + " at " + hit.position);

    }

                         // --- Stalk Functions
    /// <summary>
    ///  AI - Function 0.
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
    /// AI - Function 1.
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

                // NEW: Validate the point by casting a ray from the player to the hit point.
                // Raise the Y value slightly so the raycast doesn't scrape the floor.
                Vector3 rayStart = player.position + Vector3.up * 1f;
                Vector3 rayTarget = hit.position + Vector3.up * 1f;
                Vector3 direction = rayTarget - rayStart;
                float distance = direction.magnitude;


                // If ray DOES NOT hit anything (like a wall) , its valid point in the same room
                // Valid NavMesh point near player
                if (!Physics.Raycast(rayStart, direction.normalized, distance))
                {
                    return hit.position;   // Valid NavMesh point with clear line of sight to player
                }
            }
        }

        // Fallback: go directly to player
        return player.position;
    }


    /// <summary>
    ///  AI - Function 2.
    ///  POSITION STALK PATH
    /// </summary>
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


    /// <summary>
    ///  AI - Function 3.
    ///  Improved candidate selection: navmesh sample -> LOS check -> path completeness
    /// </summary>
    Vector3 GetAValidPointNearPlayer(float radius)
    {
        // attempt many times
        for (int i = 0; i < 30; i++)
        {
            Vector2 circle = Random.insideUnitCircle.normalized * Random.Range(radius * 0.5f, radius);
            Vector3 randomPoint = player.position + new Vector3(circle.x, 0f, circle.y);

            NavMeshHit hit;
            // use a slightly larger sample radius to catch near edges
            if (!NavMesh.SamplePosition(randomPoint, out hit, 3.0f, NavMesh.AllAreas))
                continue;

            Vector3 candidate = hit.position;

            // 1) candidate must have a path from agent (complete)
            NavMeshPath testPath = new NavMeshPath();
            if (!agent.CalculatePath(candidate, testPath) || testPath.status != NavMeshPathStatus.PathComplete)
                continue;

            // 2) candidate should have direct line-of-sight to the player (no walls)
            // Ray from candidate -> player
            Vector3 start = candidate + Vector3.up * 1.0f;   // raise off floor slightly
            Vector3 target = player.position + Vector3.up * 1.0f;
            if (!HasLineOfSight(start, target))
            {
                // candidate blocked by environment, reject
                continue;
            }

            // success
            return candidate;
        }

        // fallback: return a point near player but clamp to navmesh sample
        NavMeshHit fallback;
        if (NavMesh.SamplePosition(player.position, out fallback, 2.0f, NavMesh.AllAreas))
            return fallback.position;

        return player.position;
    }


    // --- HELPERS
    /// <summary>
    /// HELPER - Function 0.
    /// helper: set destination but do not allow agent to continue if within stoppingDistance
    /// </summary>
    void SetAgentDestinationWithStop(Vector3 dest)
    {
        float dist = Vector3.Distance(transform.position, dest);                        // transform is the Ai - not using a inspector Transform
        if (dist <= agent.stoppingDistance + Enemy_Distance)                            // float value is distance value of which it stops
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(dest);
        }
    }

    /// <summary>
    /// HELPER - Function 1.
    /// set destination but do not allow agent to continue if within stoppingDistance ensure agent won't walk into the player
    /// </summary>
    void EnforceStoppingDistance()
    {
        if (player == null) return;
        float dist = Vector3.Distance(transform.position, player.position);
        if (dist <= agent.stoppingDistance)
        {
            // stop moving and optionally look at the player
            if (!agent.isStopped)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }
        }
        else
        {
            // resume if previously stopped (but don't force Re-setDestination every frame)
            if (agent.isStopped)
                agent.isStopped = false;
        }
    }


    // Raycast helper using environmentMask. Returns true when no blocking hit found (i.e., direct sight)
    bool HasLineOfSight(Vector3 from, Vector3 to)
    {
        Vector3 dir = to - from;
        float dist = dir.magnitude;
        if (dist <= 0.001f) return true;

        RaycastHit hit;
        // Only check collisions against environmentMask (set this to walls/doors in inspector)
        if (Physics.Raycast(from, dir.normalized, out hit, dist, environmentMask, QueryTriggerInteraction.Ignore))
        {
            // NEW: Check if what we hit is actually an obstacle, not the player or ourselves
            if (hit.transform != player && hit.transform != transform)
            {
                PrintTools.Print("It hit a wall or prop!");
                return false;   // It hit a wall or prop! hit something before reaching the target: blocked
            }

        }

        return true;  // Clear sight (or only hit the player)
    }



    /// <summary>
    /// AI - Function 3 (Refined).
    /// Uses NavMesh.Raycast to ensure the point is in the same room, 
    /// then Physics.Raycast to ensure no small props are blocking the view.
    /// </summary>
    Vector3 GetRefinedStalkPoint(float radius)
    {
        for (int i = 0; i < 30; i++)
        {
            // Pick a random spot
            Vector2 circle = Random.insideUnitCircle.normalized * Random.Range(radius * 0.5f, radius);
            Vector3 randomPoint = player.position + new Vector3(circle.x, 0, circle.y);

            NavMeshHit hit;
            // 1. Is this point on the floor?
            if (NavMesh.SamplePosition(randomPoint, out hit, 3.0f, NavMesh.AllAreas))
            {
                // 2. NAVMESH FLOOR CHECK: Is there a wall boundary between player and point?
                // NavMesh.Raycast returns TRUE if it HITS a wall. We want FALSE.
                NavMeshHit navHit;
                if (!NavMesh.Raycast(player.position, hit.position, out navHit, NavMesh.AllAreas))
                {
                    // 3. SIGHT CHECK: Is there a prop/environment object in the way?
                    Vector3 eyeLevelStart = hit.position + Vector3.up * 1.5f;
                    Vector3 eyeLevelTarget = player.position + Vector3.up * 1.5f;

                    if (HasLineOfSight(eyeLevelStart, eyeLevelTarget))
                    {
                        // 4. PATH CHECK: Can the AI actually walk here?
                        NavMeshPath testPath = new NavMeshPath();
                        if (agent.CalculatePath(hit.position, testPath) && testPath.status == NavMeshPathStatus.PathComplete)
                        {
                            return hit.position; // Success!
                        }
                    }
                }
            }
        }

        // Fallback if no perfect spot is found in 30 tries
        return player.position;
    }



}   // END



/// ------------- OLD CODE
///Vector3 randomSpotNearPlayer = player.position + (Random.insideUnitSphere * 10f);
///if (NavMesh.SamplePosition(randomSpotNearPlayer, out hit, 10f, NavMesh.AllAreas))
///{
///    agent.SetDestination(hit.position);
///}