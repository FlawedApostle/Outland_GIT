using UnityEngine;
using UnityEngine.AI;

public static class NavDebugger
{
    // Returns the total "radius" (size) of the current NavMesh
    public static float GetWorldRadius()
    {
        NavMeshTriangulation tri = NavMesh.CalculateTriangulation();
        if (tri.vertices.Length == 0) return 0f;

        Bounds bounds = new Bounds(tri.vertices[0], Vector3.zero);
        foreach (Vector3 point in tri.vertices) bounds.Encapsulate(point);

        // Returns half the diagonal size of the map
        return bounds.extents.magnitude;
    }

    // Returns the absolute CENTER of your map (useful if your level isn't at 0,0,0)
    public static Vector3 GetWorldCenter()
    {
        NavMeshTriangulation tri = NavMesh.CalculateTriangulation();
        if (tri.vertices.Length == 0) return Vector3.zero;

        Bounds bounds = new Bounds(tri.vertices[0], Vector3.zero);
        foreach (Vector3 point in tri.vertices) bounds.Encapsulate(point);

        return bounds.center;
    }

    /// THis is the navmesh drawn with color 
    public static void DebugAgentPath(NavMeshAgent agent, Color color, float duration = 0f)
    {
        if (agent == null) return;
        NavMeshPath debugPath = agent.path;
        for (int i = 1; i < debugPath.corners.Length; i++)
        {
            Debug.DrawLine(debugPath.corners[i - 1], debugPath.corners[i], color, duration);  // Color.limeGreen // 1.0f 
        }
    }

    public static void DrawNavMeshTriangles(Color color, float duration = 0f)
    {
        var tri = NavMesh.CalculateTriangulation();
        var verts = tri.vertices;
        var inds = tri.indices;

        if (verts == null || inds == null) return; for (int i = 0; i < inds.Length; i += 3)
        {
            Vector3 v0 = verts[inds[i]]; Vector3 v1 = verts[inds[i + 1]];
            Vector3 v2 = verts[inds[i + 2]];
            Debug.DrawLine(v0, v1, color, duration);
            Debug.DrawLine(v1, v2, color, duration);
            Debug.DrawLine(v2, v0, color, duration);
        }

    }
}