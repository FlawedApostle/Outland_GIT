using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[ExecuteAlways] // lets OnDrawGizmos show in editor and runtime
public class NavHelper : MonoBehaviour
{
    [Tooltip("0 = draw for one frame (call each frame). >0 = seconds to persist.")]
    public float duration = 0f;
    public bool drawTriangles = true;
    
    [Header("Agent Settings")]
    public NavMeshAgent agent;
    public bool drawAgentPath = true;
    public Color agentPathColor = Color.darkRed;
    //public Color agentPathColor = new Color(0.2f, 0.8f, 0.2f); // lime-ish

    [Header ("Map Visuals")]
    public bool drawLines = false;
    public Color navmeshColor = Color.blue;
    
    // Generate Points
    [Header("Point Cloud Testing")]
    public bool showPointSamples = false;
    private List<Vector3> cachedPoints = new List<Vector3>();

    // -----------------------------------------------------
    // INSPECTOR BUTTONS (Right-Click)
    // -----------------------------------------------------

    // Function 1. GetAllNavMeshPoints - Right Click In Inspector
    [ContextMenu("Generate Point Samples")]
    public void GenerateSamples()
    {
        cachedPoints = NavDebugger.GetAllNavMeshPoints(3); // 3 samples per tri is plenty
        Debug.Log($"Generated {cachedPoints.Count} points from NavMesh.");
    }

    // Function 2. Recalculate NavMesh Bounds - Right Click In Inspector
    [ContextMenu("Recalculate NavMesh Bounds")]
    void RefreshBounds()
    {
        NavDebugger.ReadNavMeshBounds();
        Debug.Log($"NavMesh Center: {NavDebugger.navMeshCenter}, Radius: {NavDebugger.navMeshWorldRadius}");
    }

    [ContextMenu("Clear Point Samples")]
    public void ClearSamples() => cachedPoints.Clear();

    // -----------------------------------------------------
    // THE SWITCHBOARD
    // -----------------------------------------------------
    private void OnDrawGizmos()
    {
        if (drawLines) DrawNavMeshLines();
        if (showPointSamples) DrawPointSamples();
        if (drawAgentPath) DrawAgentPathTracing();
    }

    // -----------------------------------------------------
    // FUNCTIONS
    // -----------------------------------------------------
    
    void DrawNavMeshLines()
    {
        var tri = NavMesh.CalculateTriangulation();
        Gizmos.color = navmeshColor;
        for (int i = 0; i < tri.indices.Length; i += 3)
        {
            Vector3 v0 = tri.vertices[tri.indices[i]];
            Vector3 v1 = tri.vertices[tri.indices[i + 1]];
            Vector3 v2 = tri.vertices[tri.indices[i + 2]];
            Gizmos.DrawLine(v0, v1);
            Gizmos.DrawLine(v1, v2);
            Gizmos.DrawLine(v2, v0);
        }
    }

    // Take the generated points from NavDebugger.GetAllNavMeshPoints stored in cachedPoints and relay onto the screen
    //void DrawPointSamples()
    //{
    //    if (cachedPoints == null) return;
    //    Gizmos.color = Color.darkRed;
    //    foreach (Vector3 p in cachedPoints)
    //    {
    //        Gizmos.DrawSphere(p, 0.1f);
    //    }
    //}

    void DrawPointSamples()
    {
        if (cachedPoints == null) return;

        foreach (Vector3 p in cachedPoints)
        {
            // This checks LIVE if the point is still on the NavMesh
            if (NavMesh.SamplePosition(p, out NavMeshHit hit, 0.1f, NavMesh.AllAreas))
                Gizmos.color = Color.green; // Safe!
            else
                Gizmos.color = Color.red;   // Outside/Invalid!

            Gizmos.DrawSphere(p, 0.1f);
        }
    }

    void DrawAgentPathTracing()
    {
        if (agent == null || !agent.hasPath) return;

        Gizmos.color = agentPathColor;
        Vector3[] corners = agent.path.corners;
        for (int i = 1; i < corners.Length; i++)
        {
            Gizmos.DrawLine(corners[i - 1], corners[i]);
            Gizmos.DrawSphere(corners[i], 0.1f);
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(agent.destination, Vector3.one * 0.5f);
    }




    /// OLD
    /*
    //private void OnDrawGizmos()
    //{
    //    if (showPointSamples && cachedPoints != null)
    //    {
    //        Gizmos.color = Color.darkRed;
    //        foreach (Vector3 p in cachedPoints)
    //        {
    //            Gizmos.DrawSphere(p, 0.1f);
    //        }
    //    }
    //}
    */
    // Function 1. Draw the Path Lines - Cached Points
    //private void OnDrawGizmos()
    //{
    //    // 1. Draw the Point Cloud
    //    if (showPointSamples && cachedPoints != null)
    //    {
    //        Gizmos.color = Color.darkRed;
    //        foreach (Vector3 p in cachedPoints)
    //        {
    //            Gizmos.DrawSphere(p, 0.1f);
    //        }
    //    }

    //    // 2. Draw the Real-Time Agent Path
    //    if (drawAgentPath && agent != null && agent.hasPath)
    //    {
    //        Gizmos.color = agentPathColor;
    //        Vector3[] corners = agent.path.corners;
    //        for (int i = 1; i < corners.Length; i++)
    //        {
    //            Gizmos.DrawLine(corners[i - 1], corners[i]);
    //            Gizmos.DrawSphere(corners[i], 0.1f);
    //        }

    //        // Draw the Destination Goal
    //        Gizmos.color = Color.red;
    //        Gizmos.DrawWireCube(agent.destination, new Vector3(0.5f, 0.5f, 0.5f));
    //        Gizmos.DrawLine(agent.transform.position, agent.destination);
    //    }
    //}










    //void Update()
    //{
    //    //if duration == 0 call these every frame(Update)
    //    //if (duration <= 0)
    //    //{
    //        //if (drawTriangles) NavDebugger.DrawNavMeshTriangles(navmeshColor, duration);
    //        //if (drawAgentPath && agent != null) NavDebugger.DebugAgentPath(agent, agentPathColor, duration);
    //    //}
    //}


    //// Optional: draws when the object is selected in Editor
    //void OnDrawGizmosSelected()
    //{
    //    //if (drawTriangles) NavDebugger.DrawNavMeshTriangles(navmeshColor, 0f);
    //    //if (drawAgentPath && agent != null) NavDebugger.DebugAgentPath(agent, agentPathColor, 0f);
    //}


}       // END
