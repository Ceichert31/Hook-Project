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
    private Light secondaryLight;

    [SerializeField]
    private DayNightPreset preset;

    [SerializeField]
    private Vector3 lightRotation;

    private const float TOTAL_DAYTIME = 24f;
    private const float TOTAL_ROTATION = 360f;
    private const float MIN_ROTATION = 35f;
    private const float MAX_ROTATION = 145f;

    private void Update()
    {
        if (pauseTime)
            return;

        if (Application.isPlaying)
        {
            //Add time
            timeOfDay += Time.deltaTime * timeSpeedMultiplier;

            //Clamp
            timeOfDay %= TOTAL_DAYTIME;

            UpdateLighting(timeOfDay / TOTAL_DAYTIME);
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
        mainLight.color = preset.sunColor.Evaluate(timePercent);
        secondaryLight.color = preset.moonColor.Evaluate(timePercent);

        float finalRotation = (timePercent * TOTAL_ROTATION);
        //finalRotation = Mathf.Clamp(finalRotation, MIN_ROTATION, MAX_ROTATION);

        mainLight.transform.localRotation = Quaternion.Euler(
                    new(
                        finalRotation - lightRotation.x,
                        lightRotation.y,
                        lightRotation.z
                    )
                );
    }

    private void OnValidate()
    {
        UpdateLighting(timeOfDay / TOTAL_DAYTIME);
    }

}
