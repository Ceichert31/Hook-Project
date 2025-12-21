using UnityEngine;

public class WeatherController : MonoBehaviour
{
    [SerializeField]
    private WeatherPreset preset;

    private ParticleSystem windParticle;
    private ParticleSystem snowParticle;
    private void Start()
    {
        windParticle = transform.GetChild(0).GetComponent<ParticleSystem>();
        snowParticle = transform.GetChild(1).GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        transform.eulerAngles = new Vector3(0, preset.windRotation, 0);

        //Change force intensity for snow 
        var forceOverLifetime = snowParticle.forceOverLifetime;
        forceOverLifetime.xMultiplier = preset.snowIntensity;

        //Change gravity for snow
        var mainModule = snowParticle.main;
        mainModule.gravityModifier = preset.gravity;
    }
}