using UnityEngine;
/// <summary>
/// Manages the line renderer from the player to the hook
/// </summary>
public class HookInstance : MonoBehaviour
{
    private Transform parentObject;
    private LineRenderer ropeRenderer;

    private void Awake()
    {
        ropeRenderer = GetComponent<LineRenderer>();
        parentObject = transform.parent;
        PlaceObject(false);
    }

    private void Update()
    {
        ropeRenderer.SetPosition(0, parentObject.position);
        ropeRenderer.SetPosition(1, transform.position);
    }

    public void PlaceObject(bool isPlaced)
    {
        ropeRenderer.enabled = isPlaced;
    }
}
