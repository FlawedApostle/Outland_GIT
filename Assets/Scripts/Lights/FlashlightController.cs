using UnityEngine;

public partial class FlashlightController : MonoBehaviour
{
    [Header("Settings")]
    public Light flashlightLight; // Drag your Spot Light here
    public float brightness = 2.0f; // This is 'Intensity'
    public float distance = 20.0f;  // This is 'Range'

    private bool isOn = true;

    void Start()
    {
        // Set the initial values from your floats
        flashlightLight.intensity = brightness;
        flashlightLight.range = distance;
        flashlightLight.enabled = isOn;
    }

    void Update()
    {
        // Use 'F' key to toggle
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleFlashlight();
        }
    }

    void ToggleFlashlight()
    {
        isOn = !isOn;
        flashlightLight.enabled = isOn;

        // Optional: Add a click sound here later!
    }
}