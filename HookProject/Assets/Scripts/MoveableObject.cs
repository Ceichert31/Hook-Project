using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class MoveableObject : MonoBehaviour
{
    [SerializeField]
    private float friction = 1.0f;

    [SerializeField]
    private float speed = 3.0f;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

}