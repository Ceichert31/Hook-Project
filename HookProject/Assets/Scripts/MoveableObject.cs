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

    [SerializeField] 
    private float rotationSpeed = 0.03f;

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
    private Vector3 Target => GameManager.Instance.PlayerTransform.position;

    private float playerDist;

    private Vector3 targetDirection;

    [SerializeField] private int currentHookNum;

    private Rigidbody rb;

    private RaycastHit groundHit;

    private bool isGrounded;

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
        targetDirection = (Target - transform.position).normalized;

        //Get ground
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out groundHit, groundCastDistance);
        //Debug.DrawRay(transform.position, Vector3.down * 3, Color.yellow);

        //transform.up = new Vector3(targetDirection.x, 0, targetDirection.z);
        //Slowly lerp foward of object towards target direction

        //Improve in future

    }

    private void FixedUpdate()
    {
        if (currentHookNum <= 0) return;

        playerDist = Vector3.Distance(Target, transform.position);

        //Calculate and conduct movement using floating RB
        if (playerDist > maxDistance && playerDist > minDistance)
        {
            if (isGrounded)
            {
                UpdateMovement();
            }
            else
            {
                UpdateAirMovement();
            }
        }
            
    }

    private void UpdateMovement()
    {
        //Apply walk speed to the movement vector
        Vector3 moveForce = currentHookNum * maxSpeed * targetDirection;

        //Find the angle between the players up position and the groundHit
        float slopeAngle = Vector3.Angle(Vector3.up, groundHit.normal);

        //Set to (0, 0, 0)
        Vector3 yOffsetForce = Vector3.zero;

        //If surface angle is within max angle
        if (slopeAngle <= maxSlopeAngle)
        {
            //Find difference between ground distance and the offset
            float yOffsetError = (heightOffset - groundHit.distance);

            //Find the dot product of vector3.up and of the players velocity
            float yOffsetVelocity = Vector3.Dot(Vector3.up, rb.linearVelocity);

            //Set the offset force of the floating rigidbody
            yOffsetForce = Vector3.up * (yOffsetError * offsetStrength - yOffsetVelocity * offsetDamper);
        }
        //Calculate the combinded force between direction and offset
        Vector3 combinedForces = moveForce + yOffsetForce;

        //Calculate damping forces by multiplying the drag and player velocity
        Vector3 dampingForces = rb.linearVelocity * dragRate;

        //Add forces to rigidbody
        rb.AddForce((combinedForces - dampingForces) * (100 * Time.fixedDeltaTime));

        //Rotate towards move direction
        //transform.up = Vector3.MoveTowards(transform.up, rb.linearVelocity.normalized, Time.deltaTime * rotationSpeed);
        rb.AddTorque(rb.linearVelocity.normalized * rotationSpeed * (100 * Time.fixedDeltaTime));
    }

    private void UpdateAirMovement()
    {
        //Dangle and have transform.up be vector3.up
        //Try to move upwords towards player
        //Rotate towards move direction

        //Apply upwards force
        Vector3 moveForce = currentHookNum * maxSpeed * targetDirection;

        //Vector3 dampingForces = rb.linearVelocity * dragRate;

        //Add forces to rigidbody
        rb.AddForce((moveForce) * (100 * Time.fixedDeltaTime));

        transform.up = Vector3.MoveTowards(transform.up, Vector3.up, Time.deltaTime * rotationSpeed);
    }
}
public interface IHookable
{
    public void HookAdded();
    public void HookRemoved();
}