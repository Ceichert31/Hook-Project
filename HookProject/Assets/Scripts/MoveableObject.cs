using DG.Tweening;
using EventChannels;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
/// A physics script that can be hooked onto and dragged
/// </summary>
[RequireComponent(typeof(Rigidbody))]
public class MoveableObject : MonoBehaviour, IHookable
{
    [Header("Event Channel References")]
    [SerializeField]
    private VectorEventChannel applyForceChannel;
    [SerializeField]
    private VoidEventChannel endForceChannel;

    [Header("Physics Settings")]
    [SerializeField]
    private float maxDistance = 3f;

    [SerializeField]
    private float minDistance = 1f;

    [SerializeField]
    private float constrainDistance = 10f;

    [Tooltip(
        "The amount of force multiplied to the strength pulling the player back towards an object"
    )]
    [SerializeField]
    private float returnPlayerMultiplier = 15f;

    [SerializeField]
    private float baseSpeed = 5f;

    [SerializeField]
    private float rotationSpeed = 0.03f;

    [SerializeField]
    private float torque = 500;

    [Tooltip("The amount of addition speed given per hook")]
    [SerializeField]
    private float hookNumberMultiplier = 10f;

    [Tooltip("The minimum amount of velocity required for an object to start rotating")]
    [SerializeField]
    private float minRequiredVelocity = 1;

    [Header("Floating RB Settings")]
    [SerializeField]
    private float groundCastDistance = 0.7f;

    [SerializeField]
    private float heightOffset = 0.5f;

    [SerializeField]
    private float maxSlopeAngle = 45f;

    [SerializeField]
    private float offsetStrength = 100f;

    [SerializeField]
    private float offsetDamper = 10f;

    [SerializeField]
    private Transform hookAnchorPoint;

    [SerializeField]
    private Ease easeMode;

    //Debug
    [SerializeField]
    private int currentHookNum;

    [SerializeField]
    private bool enableDebug;

    [ShowIf(nameof(enableDebug))]
    [SerializeField]
    private bool showTorqueDebug;

    [ShowIf(nameof(enableDebug))]
    [SerializeField]
    private bool showDistanceDebug;

    [Header("Audio Settings")]
    [SerializeField]
    private RangedFloat pitchValues;
    private VectorEvent applyForceEvent;
    private VoidEvent endForceEvent;

    private RaycastHit groundHit;

    private bool isGrounded;
    private bool isMoving;

    private float playerDist;

    private bool playerIsTooFar;

    private Rigidbody rb;

    private ParticleSystem snowDisplacementParticle;

    private AudioSource source;

    private Vector3 targetDirection;

    private static Vector3 Target
    {
        get
        {
            return GameManager.Instance.PlayerTransform.position;
        }
    }
    
    private const float SNOW_AUDIO_CUTOFF = 0.4f;

    private void Start()
    {
        source = GetComponent<AudioSource>();
        rb = GetComponent<Rigidbody>();
        snowDisplacementParticle = GetComponentInChildren<ParticleSystem>();
    }

    private void Update()
    {
        //Prevent updates when not hooked
        if (currentHookNum <= 0)
            return;

        //Calculate player direction when hooked
        targetDirection = (Target - transform.position).normalized;

        //Get ground
        isGrounded = Physics.Raycast(
            transform.position,
            Vector3.down,
            out groundHit,
            groundCastDistance
        );

        PlaySledAudio();
        CheckPlayerDistance();
    }

    private void FixedUpdate()
    {
        if (currentHookNum <= 0)
            return;

        if (playerDist <= maxDistance || playerDist <= minDistance)
            return;

        UpdateMovement();
    }

    private void OnDrawGizmos()
    {
        if (!enableDebug)
            return;

        Debug.DrawRay(transform.position, transform.up * 3f, Color.red);
        Debug.DrawRay(transform.position, transform.forward * 3f, Color.yellow);

        //Gizmos.color = Color.blue;
        //Gizmos.DrawSphere(centerOfMass, 0.3f);
    }

    /// <summary>
    ///     Lets the object know a hook as been added
    /// </summary>
    public void HookAdded(Transform hook)
    {
        if (currentHookNum > 0)
            return;

        //Add to hook num
        currentHookNum++;

        if (currentHookNum == 1)
        {
            snowDisplacementParticle.Play();
        }

        hook.GetComponent<HookInstance>().EnableLineRenderer(true);

        //Move hook from player to sled anchor point
        hook.parent = hookAnchorPoint;
        hook.eulerAngles = new Vector3(0, -90, 0);
        hook.DOLocalMove(Vector3.zero, 0.3f).SetEase(easeMode);
        CameraShakeManager.Instance.ShakeCamera(0.3f, 0.3f, easeMode);
    }
    public void HookRemoved(Transform hook)
    {
        if (currentHookNum <= 0) return;
        
        currentHookNum--;
        
        snowDisplacementParticle.Stop();
        
        hook.GetComponent<HookInstance>().ResetHookParent();
    }

    /*/// <summary>
    ///     Lets an object know a hook has been removed
    /// </summary>
    [Button("Remove Hook")]
    public void HookRemoved()
    {
        //Remove from hook num and confirm it isn't negative

        if (currentHookNum <= 0)
            return;

        currentHookNum--;

        //hook.GetComponent<HookInstance>().PlaceObject(true);

        if (currentHookNum == 0)
        {
            snowDisplacementParticle.Stop();
        }
    }*/

    private void UpdateMovement()
    {
        //Add forces to rigidbody
        rb.AddForce(CalculateForce(), ForceMode.Force);

        //Prevent rotation when under a certain velocity
        if (rb.linearVelocity.magnitude <= minRequiredVelocity)
            return;

        //Apply torque
        rb.AddTorque(CalculateTorque(), ForceMode.Force);
    }

    /// <summary>
    ///     Calculates the amount of force necessary to move forward
    /// </summary>
    /// <returns></returns>
    private Vector3 CalculateForce()
    {
        //Apply walk speed to the movement vector
        Vector3 moveForce = (baseSpeed + currentHookNum * hookNumberMultiplier) * targetDirection;

        //Find the angle between the players up position and the groundHit
        float slopeAngle = Vector3.Angle(Vector3.up, groundHit.normal);

        //Set to (0, 0, 0)
        Vector3 yOffsetForce = Vector3.zero;

        //If surface angle is within max angle
        if (slopeAngle <= maxSlopeAngle)
        {
            //Find difference between ground distance and the offset
            float yOffsetError = heightOffset - groundHit.distance;

            //Find the dot product of vector3.up and of the players velocity
            float yOffsetVelocity = Vector3.Dot(Vector3.up, rb.linearVelocity);

            //Set the offset force of the floating rigidbody
            yOffsetForce =
                Vector3.up * (yOffsetError * offsetStrength - yOffsetVelocity * offsetDamper);
        }
        //Calculate the combined force between direction and offset
        Vector3 combinedForces = moveForce + yOffsetForce * (100 * Time.fixedDeltaTime);

        return combinedForces;
    }

    /// <summary>
    ///     Calculates the amount of torque needed to rotate towards the player
    /// </summary>
    /// <returns></returns>
    private Vector3 CalculateTorque()
    {
        //Get angle between player position an objects forward position
        float angle = Vector2.SignedAngle(
            new Vector2(transform.forward.x, transform.forward.z),
            new Vector2(targetDirection.x, targetDirection.z)
        );

        //Calculate the amount of torque force
        float torqueForce =
            (transform.rotation.y - angle) * rotationSpeed * (100 * Time.fixedDeltaTime);

        //Debug
        if (showTorqueDebug)
        {
            Debug.Log($"Angle: {angle}\n TorqueForce: {torqueForce}");
            Debug.DrawRay(transform.position, targetDirection * 3f, Color.blue);
        }

        return torque * torqueForce * Vector3.up;
    }

    /// <summary>
    ///     Checks whether the player is outside the sled's range
    ///     and pulls the player back if true
    /// </summary>
    private void CheckPlayerDistance()
    {
        //Check if player is within moving distance
        playerDist = Vector3.Distance(Target, transform.position);

        if (showDistanceDebug)
        {
            Debug.Log($"Distance to Object {gameObject.name}: {playerDist}");
        }

        //Prevent player from moving too far from the object
        if (playerDist >= constrainDistance)
        {
            playerIsTooFar = true;
            //Request force be applied to player
            //Amount of force = mass
            //-targetDirection
            applyForceEvent.Value = rb.mass * returnPlayerMultiplier * -targetDirection;

            applyForceChannel.CallEvent(applyForceEvent);
        }
        else if (playerDist <= constrainDistance && playerIsTooFar)
        {
            playerIsTooFar = false;

            endForceChannel.CallEvent(endForceEvent);

            //Request clearing of forces
        }
    }

    /// <summary>
    ///     Determines whether the sled is moving and plays a dragging sound effect
    /// </summary>
    private void PlaySledAudio()
    {
        if (rb.linearVelocity.magnitude > SNOW_AUDIO_CUTOFF && !isMoving)
        {
            isMoving = true;
            source.pitch = Random.Range(pitchValues.minValue, pitchValues.maxValue);
            source.Play();
        }
        else if (rb.linearVelocity.magnitude <= SNOW_AUDIO_CUTOFF)
        {
            isMoving = false;
            source.Stop();
        }
    }
}

public interface IHookable
{
    public void HookAdded(Transform hook);
    public void HookRemoved(Transform hook);
}
