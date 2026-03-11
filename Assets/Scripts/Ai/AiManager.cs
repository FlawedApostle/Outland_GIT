using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class AiManager : MonoBehaviour
{
    public Transform player_user;           // User Player Position
    public Transform player_enemy;         // Enemy Position
    // NavMesh
    float navMeshWorldRadius;
    Vector3 navMeshCenter;
    public NavMeshAgent agent;


    void Start ()
    {

    }

    void Update()
    {
        NavMeshPathTest(); DebugSampleEndpoints(); ReadNavMeshBounds();
    }


    // Path Debugging
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

    // Sample Positions of player & enemy
    void DebugSampleEndpoints()
    {
        NavMeshHit hitStart;
        NavMeshHit hitEnd;

        bool startOK = NavMesh.SamplePosition(transform.position, out hitStart, 0.5f, NavMesh.AllAreas);
        bool endOK = NavMesh.SamplePosition(player_user.position, out hitEnd, 0.5f, NavMesh.AllAreas);

        Debug.Log($"Sample START: {startOK} at {hitStart.position}");
        Debug.Log($"Sample END:   {endOK} at {hitEnd.position}");
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
        PrintTools.Print("Distance is inside = True", "red");                                        // Debug
        return distance <= navMeshWorldRadius;
    }


}       // END
