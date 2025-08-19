using System.Collections;
using System.Collections.Generic;
using EventChannels;
using UnityEngine;
using UnityEngine.Rendering;

[ExecuteInEditMode]
public class DayNightCycle : MonoBehaviour
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
    private Vector3 lightRotation;

    [SerializeField]
    private Light sunLight;

    [SerializeField]
    //private Light moonLight;

    private Coroutine instance;

    private BoolEvent isDayTime;

    private float advanceTimeTimer;

    //Consts
    private const float TOTALROTATION = 360f;

    private const float TOTALDAYTIME = 24f;

    private const float SUNRISE = 6f;

    private const float SUNSET = 17.5f;

    private const float ADVANCETIMEDELAY = 0.5f;

    private void Awake()
    {
        sunLight = transform.GetChild(0).GetComponent<Light>();

        //moonLight = transform.GetChild(0).GetChild(0).GetComponent<Light>();

        timeOfDay = defaultTimeOfDay;
    }

    private void Update()
    {
        if (pauseTime)
            return;

        if (Application.isPlaying)
        {
            //Add time
            timeOfDay += Time.deltaTime * timeSpeedMultiplier;

            //Clamp
            timeOfDay %= TOTALDAYTIME;

            UpdateLighting(timeOfDay / TOTALDAYTIME);
        }
    }

    private void UpdateLighting(float timePercent)
    {
        //moonLight.color = preset.moonLightColor.Evaluate(timePercent);

        //RenderSettings.fogColor = preset.fogColor.Evaluate(timePercent);

        //RenderSettings.ambientLight = preset.ambientLight.Evaluate(timePercent);

        if (sunLight != null)
        {
            //sunLight.color = preset.directionalColor.Evaluate(timePercent);

            sunLight.transform.localRotation = Quaternion.Euler(
                new(
                    (timePercent * TOTALROTATION) - lightRotation.x,
                    lightRotation.y,
                    lightRotation.z
                )
            );
        }

        if (instance != null)
            return;

        //Intensity based on time of day
        if (timeOfDay > SUNRISE && timeOfDay < SUNSET)
        {
            SetDayTimeValues();
        }
        else
        {
            SetNightTimeValues();
        }
    }

    private void OnValidate()
    {
        UpdateLighting(timeOfDay / TOTALDAYTIME);
    }

    /// <summary>
    /// Runs everything related to daytime functionallity
    /// </summary>
    private void SetDayTimeValues()
    {
        isDayTime.Value = true;
        //time_EventChannel.CallEvent(isDayTime);
    }

    /// <summary>
    /// Runs everything related to nighttime functionallity
    /// </summary>
    private void SetNightTimeValues()
    {
        isDayTime.Value = false;
        //time_EventChannel.CallEvent(isDayTime);
    }

    IEnumerator SetAmbientLightIntensity(float targetIntensity)
    {
        while (RenderSettings.ambientIntensity != targetIntensity)
        {
            //RenderSettings.ambientIntensity = Mathf.MoveTowards(
            //    moonLight.intensity,
            //    targetIntensity,
            //    Time.deltaTime
            //);

            yield return null;
        }

        RenderSettings.ambientIntensity = targetIntensity;

        instance = null;
    }

    /// <summary>
    /// Pauses and unpauses day/night cycle progression
    /// </summary>
    /// <param name="ctx"></param>
    public void PauseTime(BoolEvent ctx)
    {
        pauseTime = ctx.Value;
    }

    public void StartTimeTransition(VoidEvent ctx)
    {
        if (advanceTimeTimer > Time.time)
            return;

        advanceTimeTimer = Time.time + ADVANCETIMEDELAY;

        //advanceTimeSequencer.InitializeSequence();
    }

    /// <summary>
    /// Advances time by a certain amount
    /// </summary>
    /// <param name="ctx"></param>
    public void AdvanceTime(VoidEvent ctx)
    {
        if (isDayTime.Value)
            timeOfDay = SUNSET;
        else
            timeOfDay = SUNRISE;
    }

    /// <summary>
    /// Sets the density of unity fog
    /// </summary>
    /// <param name="ctx"></param>
    public void SetFogDensity(FloatEvent ctx)
    {
        RenderSettings.fogDensity = ctx.FloatValue;
    }
}
