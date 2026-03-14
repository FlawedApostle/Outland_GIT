using UnityEngine;

/* HOW TO USE PRINT TOOLS:
  1. Print(val) -> Standard log
  2. Print(val, "red") -> Colored value
  3. Print("Label", val) -> Label: [Red Value]
  4. Print("Label", val, "green") -> [Green Label]: [Green Value]
  5. Print(val, "blue", "TITLE") -> TITLE (new line) [Blue Value]
*/

public static class PrintTools
{
    public static void Print(object value)
    {
        UnityEngine.Debug.Log(value);
    }

    public static void Print(object value , string color)
    {
        UnityEngine.Debug.Log($"<color={color}>{value}</color>");
    }

    public static void Print(string label, object value) 
    { 
        UnityEngine.Debug.Log($"{label}: <color=red>{value}</color>");
    }

    public static void Print(string label, string color) 
    { 
        UnityEngine.Debug.Log($"<color={color}>{label}</color>");
    }

    public static void Print(string label, object value , string color)
    {
        UnityEngine.Debug.Log($"<color={color}>{label}</color>: <color={color}>{value}</color>");
    }

   // Using unity debugging logs
    public static void PrintLog(object value, string log, string color)
    {

        UnityEngine.Debug.Log(log + "\n" + $"<color={color}>{value}</color>");
    }


    // Get World Nav Mesh Radius - relying on NavMesh script placed in world
    public static void ReportWorldSize()
    {
        float radius = NavDebugger.GetWorldRadius();
        Vector3 center = NavDebugger.GetWorldCenter();

        // Using your 3-parameter print style: Value, Title, Color
        Print(radius.ToString("F2"), "--- NAVMESH RADIUS ---", "cyan");
        Print(center.ToString(), "--- NAVMESH CENTER ---", "yellow");
    }

}

// jess wed conditioning