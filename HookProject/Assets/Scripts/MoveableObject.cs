using System;
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
    private float baseSpeed = 5f;
    [SerializeField] 
    private float rotationSpeed = 0.03f;
    [SerializeField]
    private float torque = 500;

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

    //Debug
    [SerializeField] 
    private int currentHookNum;

    private float playerDist;

    private Vector3 targetDirection;

    private Rigidbody rb;

    private RaycastHit groundHit;

    private bool isGrounded;
    [SerializeField]
    private bool enableDebug;

    private Vector3 Target => GameManager.Instance.PlayerTransform.position;

    private const float VELOCITY_MIN = 2f;

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
    }

    private void FixedUpdate()
    {
        if (currentHookNum <= 0) return;

        //Check if player is within moving distance
        playerDist = Vector3.Distance(Target, transform.position);

        if (enableDebug)
        {
            Debug.Log($"Distance to Object {gameObject.name}: {playerDist}");
        }

        if (playerDist <= maxDistance || playerDist <= minDistance) return;

        UpdateMovement();

        //Check whether the object is grounded or not and apply according logic
        //if (isGrounded)
        //{
        //    UpdateMovement();
        //}
        //else
        //{
        //    UpdateAirMovement();
        //}
    }
    private void UpdateMovement()
    {
        //Add forces to rigidbody
        rb.AddForce(CalculateForce(), ForceMode.Force);

        //Prevent rotation when under a certain velocity
        if (rb.linearVelocity.magnitude <= VELOCITY_MIN) return;

        //Apply torque
        rb.AddTorque(CalculateTorque(), ForceMode.Force);
    }

    /// <summary>
    /// Calculates the amount of force nessecary to move forward
    /// </summary>
    /// <returns></returns>
    Vector3 CalculateForce()
    {
        //Apply walk speed to the movement vector
        Vector3 moveForce = currentHookNum * baseSpeed * targetDirection;

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
        Vector3 combinedForces = moveForce + yOffsetForce * (100 * Time.fixedDeltaTime);

        return combinedForces;
    }

    /// <summary>
    /// Calculates the amount of torque needed to rotate towards the player
    /// </summary>
    /// <returns></returns>
    Vector3 CalculateTorque()
    {
        //Get angle between player position an objects forward position
        float angle = Vector2.SignedAngle(new Vector2(transform.up.x, transform.up.z), new Vector2(targetDirection.x, targetDirection.z));

        //Calculate the amount of torque force
        float torqueForce = (transform.rotation.y - angle) * rotationSpeed * (100 * Time.fixedDeltaTime);

        //Debug
        if (enableDebug)
        {
            Debug.Log($"Angle: {angle}\n TorqueForce: {torqueForce}");
            Debug.DrawRay(transform.position, targetDirection * 3f, Color.blue);
            Debug.DrawRay(transform.position, transform.up * 3f, Color.red);
            Debug.DrawRay(transform.position, transform.forward * 3f, Color.yellow);
        }
        
        return (torque * torqueForce) * Vector3.up;
    }

    private void UpdateAirMovement()
    {
        //Dangle and have transform.up be vector3.up
        //Try to move upwords towards player
        //Rotate towards move direction

        //Apply upwards force
        Vector3 moveForce = currentHookNum * baseSpeed * targetDirection;

        //Vector3 dampingForces = rb.linearVelocity * dragRate;

        //Add forces to rigidbody
        rb.AddForce(moveForce * (100 * Time.fixedDeltaTime));

        //transform.up = Vector3.MoveTowards(transform.up, Vector3.up, Time.deltaTime * rotationSpeed);
    }
}
public interface IHookable
{
    public void HookAdded();
    public void HookRemoved();
}