using UnityEngine;
using UnityEngine.AI;

[ExecuteAlways] // lets OnDrawGizmos show in editor and runtime
public class NavHelper : MonoBehaviour
{
    public NavMeshAgent agent;
    public bool drawTriangles = true;
    public Color navmeshColor = Color.green;
    public bool drawAgentPath = true;
    public Color agentPathColor = new Color(0.2f, 0.8f, 0.2f); // lime-ish
    [Tooltip("0 = draw for one frame (call each frame). >0 = seconds to persist.")]
    public float duration = 0f;

    void Update()
    {
        // if duration == 0 call these every frame (Update)
        if (drawTriangles) NavDebugger.DrawNavMeshTriangles(navmeshColor, duration);
        if (drawAgentPath && agent != null) NavDebugger.DebugAgentPath(agent, agentPathColor, duration);
    }

    // Optional: draws when the object is selected in Editor
    void OnDrawGizmosSelected()
    {
        if (drawTriangles) NavDebugger.DrawNavMeshTriangles(navmeshColor, 0f);
        if (drawAgentPath && agent != null) NavDebugger.DebugAgentPath(agent, agentPathColor, 0f);
    }
}
