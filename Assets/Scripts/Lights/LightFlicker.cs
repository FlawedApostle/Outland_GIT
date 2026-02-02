using System.Threading;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    private Light localLight;
    public float minIntensity = 0.2f; // Dimness
    public float maxIntensity = 1.2f; // Brightness
    public float flickerSpeed = 0.05f; // How fast it stutters



    void Start()
    {
        localLight = GetComponent<Light>();
        // Start the flickering loop
        InvokeRepeating("Flicker", 0, flickerSpeed);
    }

    void Flicker()
    {
        // Pick a random brightness between our min and max
        localLight.intensity = Random.Range(minIntensity, maxIntensity);
    }
}