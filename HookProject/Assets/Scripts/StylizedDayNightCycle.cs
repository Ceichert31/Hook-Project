using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class StylizedDayNightCycle : MonoBehaviour
{
    [Header("Day Night Settings")]
    [SerializeField]
    private bool pauseTime;

    [Range(0f, 24f)]
    [SerializeField]
    private float timeOfDay;

    [SerializeField]
    private float timeSpeedMultiplier = 0.5f;

    [SerializeField]
    private float defaultTimeOfDay = 4f;

    [SerializeField]
    private float advanceTimeBy = 12f;

    [SerializeField]
    private Light mainLight;

    [SerializeField]
    private DayNightPreset preset;

    private const float TOTALDAYTIME = 24f;

    private void Update()
    {
        if (pauseTime)
            return;

        if (Application.isPlaying)
        {
            //Add time
            timeOfDay += (100 * Time.deltaTime) * timeSpeedMultiplier;

            //Clamp
            timeOfDay %= TOTALDAYTIME;

            UpdateLighting(timeOfDay / TOTALDAYTIME);
        }
    }

    /// <summary>
    /// Updates the skybox and fog color based on time of day
    /// </summary>
    /// <param name="timePercent"></param>
    private void UpdateLighting(float timePercent)
    {
        //Set fog and camera skybox color
        RenderSettings.fogColor = preset.fogColor.Evaluate(timePercent);
        RenderSettings.ambientLight = preset.ambientLightColor.Evaluate(timePercent);
        Camera.main.backgroundColor = preset.SkyboxColor.Evaluate(timePercent);
        mainLight.color = preset.lightingColor.Evaluate(timePercent);
    }

    private void OnValidate()
    {
        UpdateLighting(timeOfDay / TOTALDAYTIME);
    }

}
