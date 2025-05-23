using UnityEngine;

public class Headbob : MonoBehaviour
{
    [Header("Headbob Refrences")]
    [Tooltip("Camera holder which will have movement applied to it")]
    [SerializeField] private Transform headbobTarget;   

    [Header("Headbob Settings")]
    [Tooltip("How fast the x and y of the camera bob")]
    [SerializeField] private Vector2 headbobSpeed;

    [Tooltip("How far the x and y of the camera will bob")]
    [SerializeField] private Vector2 headbobIntensity;

    [Tooltip("The vertical path the camera will take when bobbing")]
    [SerializeField] private AnimationCurve headbobCurveY;

    [Tooltip("The horizontal path the camera will take when bobbing")]
    [SerializeField] private AnimationCurve headbobCurveX;

    private InputController inputController;
    private Rigidbody rb;

    private Vector2 currentPos;
    private Vector2 currentTime;
    private Vector3 velocity;

    private float smoothTime;

    private bool applyMovementEffects => inputController.ApplyMovementEffects;
    private bool isGrounded => inputController.IsGrounded;
    private bool isMoving => inputController.IsMoving;

    private void Start()
    {
        inputController = GetComponent<InputController>();

        rb = GetComponent<Rigidbody>();

        if (headbobTarget == null)
            headbobTarget = transform.GetChild(0);
    }
    private void Update()
    {
        if (applyMovementEffects) return;

        UpdateBob();
    }

    void UpdateBob()
    {
        float speedFactor = rb.linearVelocity.magnitude;

        if (!isGrounded || !isMoving)
        {
            currentPos = Vector2.zero;
            currentTime = Vector2.zero;
            smoothTime = 0.2f;
        }
        else if (isMoving)
        {
            currentTime.x += headbobSpeed.x / 10 * Time.deltaTime * speedFactor;
            currentTime.y += headbobSpeed.y / 10 * Time.deltaTime * speedFactor;
            currentPos.x = headbobCurveX.Evaluate(currentTime.x) * headbobIntensity.x;
            currentPos.y = headbobCurveY.Evaluate(currentTime.y) * headbobIntensity.y;

            smoothTime = 0.1f;
        }
    }
    private void FixedUpdate()
    {
        Vector3 targetPos = new(currentPos.x, currentPos.y, 0);
        Vector3 desiredPos = Vector3.SmoothDamp(headbobTarget.localPosition, targetPos, ref velocity, smoothTime);

        headbobTarget.localPosition = desiredPos;
    }
}
