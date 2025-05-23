using UnityEngine;

public class HookInstance : MonoBehaviour
{
    private LineRenderer ropeRenderer;

    private Transform parentObject;

    private void Awake()
    {
        ropeRenderer = GetComponent<LineRenderer>();
        parentObject = transform.parent;
    }

    private void Update()
    {
        ropeRenderer.SetPosition(0, parentObject.position);
        ropeRenderer.SetPosition(1, transform.position);
    }
}
