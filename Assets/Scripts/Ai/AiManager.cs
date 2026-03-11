using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class AiManager : MonoBehaviour
{
    public Animator animatorScript;
    public Transform player_user;                       // User Player Position
    public Transform player_enemy;                      // Enemy Position
    // NavMesh
    float navMeshWorldRadius;
    Vector3 navMeshCenter;
    public NavMeshAgent agent;                          // This also checks the NavMeshPath Path , Using the agent we can check both



    void Start ()
    {
        ReadNavMeshBounds();                            // expensive LEAVE  in start
    }

    void Update()
    {
        NavMeshPathTest();  DebugAgentPath(agent);                               
        // DebugSampleEndpoints();  /Position_Player(player_user, player_enemy); 
        Direction_ForwardVector(player_user);
        Direction_ToEnemy(player_user, player_enemy);
    }

    /// Is the player facing the enemy
    void Direction_ToEnemy(Transform _player , Transform _enemy)
    {
        Vector3 dirToEnemy = (_enemy.position - _player.position).normalized;
        float dot = Vector3.Dot(_player.forward, dirToEnemy);
        //PrintTools.Print("Dot value = ", dot, "yellow");
        if (dot > 0.7f)
        {
            PrintTools.Print("Player is facing the enemy" , "green");
        }
        else
        {
            PrintTools.Print("Player is NOT facing the enemy" , "red");
        }
    }

    /// What direction is the player facing using direction Vector transform.forward
    void Direction_ForwardVector(Transform _player)
    {
        //Debug.Log("Player Forward Dir: " + player_user.forward);
        PrintTools.Print("Player Forward Direction vector: ", _player.forward, "green");
    }

    // FIX THIS - SOME REASON THE ENEMY & THE PLAYER ARE THE SAME LOCATION
    void Position_Player(Transform _player , Transform _enemy)
    {
        /// player_user
        Vector3 pos_player = _player.position;
        Vector3 pos_enemy = _player.position;
        //Debug.Log("Player Position XYZ: " + pos_player);
        //Debug.Log("Enemy Position XYZ: " + pos_enemy);
        PrintTools.Print("Player Position XYZ: ", pos_player, "green");
        PrintTools.Print("Enemy Position XYZ: ", pos_enemy, "red");
    }

    /// THis is the navmesh drawn with color
    void DebugAgentPath(NavMeshAgent agent)
    {
        // debugging 
        NavMeshPath debugPath = agent.path;
        for (int i = 1; i < debugPath.corners.Length; i++)
        {
            Debug.DrawLine(debugPath.corners[i - 1], debugPath.corners[i], Color.limeGreen, 1.0f);
        }

        //if (!agent.pathPending)
        //    Debug.Log("Agent path status: " + agent.pathStatus + ", remainingDist=" + agent.remainingDistance);
    }

    /// Path Debugging - checking whether the enemy has found a partial / failed / completed path
    void NavMeshPathTest()
    {
        NavMeshPath debugPath = new NavMeshPath();
        if (NavMesh.CalculatePath(player_enemy.position, player_user.position, NavMesh.AllAreas, debugPath))
        {
            Debug.Log("PATH STATUS = " + debugPath.status);
        }
        else
        {
            Debug.Log("CalculatePath FAILED");
        }
    }

    /// Sample Positions of player & enemy
    void DebugSampleEndpoints()
    {
        NavMeshHit hitStart;
        NavMeshHit hitEnd;

        bool startOK = NavMesh.SamplePosition(transform.position, out hitStart, 0.5f, NavMesh.AllAreas);
        bool endOK = NavMesh.SamplePosition(player_user.position, out hitEnd, 0.5f, NavMesh.AllAreas);

        Debug.Log($"Sample START: {startOK} at {hitStart.position}");
        Debug.Log($"Sample END:   {endOK} at {hitEnd.position}");
    }

    /// Read The NavMesh Bounds
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
    
    /// Check for point inside the NavMesh
    bool IsPointInsideNavMeshBounds(Vector3 point)
    {
        float distance = Vector3.Distance(point, navMeshCenter);
        PrintTools.Print("Distance is inside = True", "red");                                        // Debug
        return distance <= navMeshWorldRadius;
    }

    /// GIZMO - {so this is an odd function i need to check if I can call this, as at times I cannot,
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
   


}       // END
