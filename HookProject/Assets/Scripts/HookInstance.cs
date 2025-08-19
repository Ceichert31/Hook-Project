using UnityEngine;
public class HookInstance : MonoBehaviour
{
    private Transform _parentObject;
    private LineRenderer _ropeRenderer;

    private void Awake()
    {
        _ropeRenderer = GetComponent<LineRenderer>();
        _parentObject = transform.parent;
        PlaceObject(false);
    }

    private void Update()
    {
        _ropeRenderer.SetPosition(0, _parentObject.position);
        _ropeRenderer.SetPosition(1, transform.position);
    }

    public void PlaceObject(bool isPlaced)
    {
        _ropeRenderer.enabled = isPlaced;
    }
}
