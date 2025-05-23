using DG.Tweening;
using UnityEngine;

public class HookController : MonoBehaviour
{
    [Header("DoTween Settings")]
    [SerializeField]
    private float hookPlaceDuration = 2f;

    [SerializeField]
    private float cameraShakeDuration = 0.3f;
    [SerializeField]
    private float cameraShakeStrength = 0.3f;

    private HookPool objectPool;

    private GameObject hookObject;

    private void Start()
    {
        objectPool = GetComponent<HookPool>();
    }

    /// <summary>
    /// Places a hook at given position
    /// </summary>
    /// <param name="ctx"></param>
    public void PlaceHook(VectorEvent ctx)
    {
        //Get available hook
        GameObject instance = objectPool.GetHook();

        //Unparent
        instance.transform.parent = null;

        //Move
        instance.transform.DOMove(ctx.Value, hookPlaceDuration).SetEase(Ease.InElastic).OnComplete(Shake);
    }

    public void DisposeHook(VoidEvent ctx)
    {
        //Remove oldest hook
        //Lerp back to holder
    }

    /// <summary>
    /// Shakes the camera
    /// </summary>
    private void Shake()
    {
        //Camera shake
        Camera.main.DOShakePosition(cameraShakeDuration, cameraShakeStrength);
    }
}
