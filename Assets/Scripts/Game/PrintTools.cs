using UnityEngine;

public static class PrintTools
{
    public static void Print(object value)
    {
        UnityEngine.Debug.Log(value);
    }

    public static void Print(string label, object value) 
    { 
        UnityEngine.Debug.Log($"{label}: <color=red>{value}</color>");
    }

    public static void Print(string label, object value , string color)
    {
        UnityEngine.Debug.Log($"{label}: <color={color}>{value}</color>");
    }
}

// jess wed conditioning