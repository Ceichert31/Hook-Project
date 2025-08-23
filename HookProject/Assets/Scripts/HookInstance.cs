using UnityEngine;
using DG.Tweening;
/// <summary>
/// Manages the line renderer from the player to the hook
/// </summary>
public class HookInstance : MonoBehaviour
{
    [SerializeField]
    private float returnTime = 0.3f;
    
    private Transform parentObject;
    private LineRenderer ropeRenderer;

    private void Awake()
    {
        ropeRenderer = GetComponent<LineRenderer>();
        parentObject = transform.parent;
        EnableLineRenderer(false);
    }

    private void Update()
    {
        ropeRenderer.SetPosition(0, parentObject.position);
        ropeRenderer.SetPosition(1, transform.position);
    }

    /// <summary>
    /// Flags whether the line renderer should be visible or not
    /// </summary>
    /// <param name="isPlaced">The visibility flag</param>
    public void EnableLineRenderer(bool isPlaced)
    {
        ropeRenderer.enabled = isPlaced;
    }
    /// <summary>
    /// Resets the hooks parent and moves the hook back to its original position
    /// </summary>
    public void ResetHookParent()
    {
        EnableLineRenderer(false);
        DOTween.CompleteAll();
        transform.parent = parentObject;
        transform.DOLocalMove(Vector3.zero, returnTime);
    }
}
