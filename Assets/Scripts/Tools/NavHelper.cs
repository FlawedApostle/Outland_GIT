using UnityEngine;
using UnityEngine.AI;

public static class NavHelper
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
}