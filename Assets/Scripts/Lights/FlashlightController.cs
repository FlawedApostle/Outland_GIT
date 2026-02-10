using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    // These will show up in your Inspector for easy tweaking
    [Header("Light Settings")]
    public Light mySpotlight;
    public float maxBrightness = 2.0f;
    public float lightRange = 20.0f;
    public float fieldOfView = 45.0f; // This is 'Spot Angle'

    private bool isLightOn = true;

    void Start()
    {
        // Automatically find the light if you forgot to drag it in
        if (mySpotlight == null)
        {
            mySpotlight = GetComponentInChildren<Light>();
        }

        // Apply your float settings
        UpdateLightSettings();
    }

    void Update()
    {
        // Toggle with F key
        if (Input.GetKeyDown(KeyCode.F))
        {
            isLightOn = !isLightOn;
            mySpotlight.enabled = isLightOn;
        }
    }

    // Call this to update the light values from the floats
    void UpdateLightSettings()
    {
        mySpotlight.intensity = maxBrightness;
        mySpotlight.range = lightRange;
        mySpotlight.spotAngle = fieldOfView;
    }
}