using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class NavDebugger
{

    public static float navMeshWorldRadius;
    public static Vector3 navMeshCenter;





    // Returns the total "radius" (size) of the current NavMesh
    public static float GetWorldRadius()
    {
        NavMeshTriangulation tri = NavMesh.CalculateTriangulation();
        if (tri.vertices.Length == 0) return 0f;

        Bounds bounds = new Bounds(tri.vertices[0], Vector3.zero);
        foreach (Vector3 point in tri.vertices) bounds.Encapsulate(point);

        // Returns half the diagonal size of the map
        //return bounds.extents.magnitude;
        // Align radii better with Rectanglular maps
        return Mathf.Max(bounds.extents.x, bounds.extents.z);
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


    // -- NAVMESH

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

    // Read The NavMesh Bounds
    public static void ReadNavMeshBounds()
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


    public static List<Vector3> GetAllNavMeshPoints(int samplesPerTriangle = 5)
    {
        var triangulation = NavMesh.CalculateTriangulation();
        var verts = triangulation.vertices;
        var indices = triangulation.indices;

        List<Vector3> points = new List<Vector3>();

        for (int i = 0; i < indices.Length; i += 3)
        {
            Vector3 a = verts[indices[i]];
            Vector3 b = verts[indices[i + 1]];
            Vector3 c = verts[indices[i + 2]];

            for (int s = 0; s < samplesPerTriangle; s++)
            {
                // Random barycentric coordinates
                float r1 = Random.value;
                float r2 = Random.value;

                // Ensure point stays inside triangle
                if (r1 + r2 > 1f)
                {
                    r1 = 1f - r1;
                    r2 = 1f - r2;
                }

                Vector3 p = a + r1 * (b - a) + r2 * (c - a);

                // Optional: project to nearest NavMesh point (ensures validity)
                if (NavMesh.SamplePosition(p, out NavMeshHit hit, 0.1f, NavMesh.AllAreas))
                    points.Add(hit.position);
            }
        }

        return points;
    }





}       // END