using UnityEngine;

public class WeatherController : MonoBehaviour
{
    [SerializeField]
    private WeatherPreset preset;

    private ParticleSystem windParticle;
    private ParticleSystem snowParticle;

    public float changeDirectionTime = 15f;
    private float changeDirectionTimer;

    private void Start()
    {
        windParticle = transform.GetChild(0).GetComponent<ParticleSystem>();
        snowParticle = transform.GetChild(1).GetComponent<ParticleSystem>();
    }

    private void Update()
    {
        Debug.DrawRay(windParticle.gameObject.transform.position, windParticle.gameObject.transform.forward * 5, Color.red);

        //Change force intensity for snow 
        var forceOverLifetime = snowParticle.forceOverLifetime;
        forceOverLifetime.xMultiplier = preset.snowIntensity;

        //Change gravity for snow
        var mainModule = snowParticle.main;
        mainModule.gravityModifier = preset.gravity;

        changeDirectionTimer += Time.deltaTime;
        if (changeDirectionTimer > changeDirectionTime)
        {
            changeDirectionTimer = 0;
            transform.Rotate(Vector3.up, preset.windRotation, Space.Self);
        }   
    }
}