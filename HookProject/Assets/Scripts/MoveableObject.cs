using NaughtyAttributes;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MoveableObject : MonoBehaviour, IHookable
{
    //Needs to know how many hooks it has 
    //Needs to know players direction
    //Min/max player distances
    //Drag forces, speed and mass
    //Use psuedo floating rigidbody system

    [Header("Physics Settings")]
    [SerializeField]
    private float maxDistance = 3f;
    [SerializeField]
    private float minDistance = 1f;

    [SerializeField]
    private float maxSpeed = 5f;
    [SerializeField]
    private float minSpeed = 1f;

    [SerializeField]
    private float dragRate = 1f;

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
    private float offsetDamper;
    private Vector3 target => GameManager.Instance.PlayerTransform.position;

    private float playerDist;

    private Vector3 targetDirection;

    [SerializeField] private int currentHookNum;

    private Rigidbody rb;

    private RaycastHit groundHit;

    private const float DIST_MODIFIER = 10f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// Lets the object know a hook as been added
    /// </summary>
    [Button("Add hook")]
    public void HookAdded()
    {
        //Add to hook num
        currentHookNum++;
    }
    /// <summary>
    /// Lets an object know a hook has been removed
    /// </summary>
    [Button("Remove Hook")]
    public void HookRemoved()
    {
        //Remove from hook num and confirm it isn't negative

        if (currentHookNum <= 0) return;

        currentHookNum--;

    }


    private void Update()
    {
        //Prevent updates when not hooked
        if (currentHookNum <= 0) return;

        //Calculate player direction when hooked
        targetDirection = (target - transform.position).normalized;

        //Get ground
        Physics.Raycast(transform.position, Vector3.down, out groundHit, groundCastDistance);
    }

    private void FixedUpdate()
    {
        if (currentHookNum <= 0) return;

        playerDist = Vector3.Distance(target, transform.position);

        //Calculate and conduct movement using floating RB
        if (playerDist > maxDistance || playerDist < minDistance)
        {
            UpdateMovement();
        }
      
    }

    private void UpdateMovement()
    {
        //Eventually replace with lerping speed
        Vector3 moveForce = (maxSpeed * targetDirection) * playerDist / DIST_MODIFIER;

        //Eventually add slope support with a groundcast
        float slopeAngle = Vector3.Angle(Vector3.up, groundHit.normal);

        Vector3 yOffsetForce = Vector3.zero;

        //Check if slope is too steep
        if (slopeAngle <= maxSlopeAngle)
        {
            float yOffsetError = (heightOffset - groundHit.distance);

            float yOffsetVelocity = Vector3.Dot(Vector3.up, rb.linearVelocity);

            yOffsetForce = Vector3.up * (yOffsetError * offsetStrength - yOffsetVelocity * offsetDamper);
        }

        Vector3 combindedForces = moveForce + yOffsetForce;
        
        //Calculate damping forces
        Vector3 dampingForces = rb.linearVelocity * dragRate;

        //Apply forces to rigidbody
        rb.AddForce((moveForce - dampingForces) * (100 * Time.fixedDeltaTime));
    }
}
public interface IHookable
{
    public void HookAdded();
    public void HookRemoved();
}