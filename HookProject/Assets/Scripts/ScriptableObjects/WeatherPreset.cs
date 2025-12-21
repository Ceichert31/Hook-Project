using UnityEngine;

[CreateAssetMenu(menuName = "Presets/Weather Preset")]
public class WeatherPreset : ScriptableObject
{
    [Header("Snow Particle Settings")]
    public float snowIntensity = 1f;
    public float gravity = 0.5f;

    public bool useRandomDirection;
    public float windRotation;

    //Todo: Implement settings for wind particle

    public WeatherPreset()
    {
        if (!useRandomDirection) return;
        windRotation = Random.Range(0, 360);
    }
}
