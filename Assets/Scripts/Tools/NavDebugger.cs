using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public static class NavDebugger
{

    public static float navMeshWorldRadius;
    public static Vector3 navMeshCenter;




    // -- NAVMESH

    // Get The NavMesh Points - Right Click In Inspector
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
                /// point = A + some amount toward B + some amount toward C
                /// (b - a) = direction from A → B , (c - a) = direction from A → C
                if (r1 + r2 > 1f) 
                { 
                    r1 = 1f - r1; r2 = 1f - r2; 
                }
                Vector3 p = a + r1 * (b - a) + r2 * (c - a);

        
                // Optional: project to nearest NavMesh point (ensures validity)
                if (NavMesh.SamplePosition(p, out NavMeshHit hit, 0.1f, NavMesh.AllAreas))
                {
                    // NEW: Check if OVerlap - Find New Point 0.5 apart
                    bool tooClose = false;
                    foreach (Vector3 existingPoint in points)
                    {
                        if (Vector3.Distance(hit.position, existingPoint) < 0.5f) // 0.5m buffer
                        {
                            tooClose = true;
                            break;
                        }
                    }

                    if (!tooClose)
                    {
                        points.Add(hit.position);
                    }
                }
            }
        }

        return points;
    }

    // Read The NavMesh Bounds - Right Click In Inspector
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
    
    /// Get The NavMesh Triangles - DEPRECATED TO NAVHELPER
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
    /// Function. Draw Line On NavMesh - DEPRECATED TO NAVHELPER
    public static void DrawNavMeshLines()
    {
        var triangulation = NavMesh.CalculateTriangulation();

        Gizmos.color = Color.blue;

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
    /// THis is the navmesh drawn with color - DEPRECATED TO NAVHELPER
    public static void DebugAgentPath(NavMeshAgent agent, Color color, float duration = 0f)
    {
        if (agent == null) return;
        NavMeshPath debugPath = agent.path;
        for (int i = 1; i < debugPath.corners.Length; i++)
        {
            Debug.DrawLine(debugPath.corners[i - 1], debugPath.corners[i], color, duration);  // Color.limeGreen // 1.0f 
        }
    }

    
    


    // Get NavMesh Total Area
    public static float GetTotalNavMeshArea()
    {
        var tri = NavMesh.CalculateTriangulation();
        float totalArea = 0f;
        for (int i = 0; i < tri.indices.Length; i += 3)
        {
            Vector3 a = tri.vertices[tri.indices[i]];
            Vector3 b = tri.vertices[tri.indices[i + 1]];
            Vector3 c = tri.vertices[tri.indices[i + 2]];
            // Triangle Area Formula: 0.5 * magnitude of cross product
            totalArea += Vector3.Cross(b - a, c - a).magnitude * 0.5f;
        }
        return totalArea;
    }
    
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



}       // END