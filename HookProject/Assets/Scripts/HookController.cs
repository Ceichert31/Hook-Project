using DG.Tweening;
using UnityEngine;

public class HookController : MonoBehaviour
{
    [Header("DoTween Settings")]
    [SerializeField]
    private float hookPlaceDuration = 2f;

    [SerializeField]
    private float hookPlaceDelay = 1.3f;

    private HookPool objectPool;

    private float hookPlaceTimer;

    private void Start()
    {
        objectPool = GetComponent<HookPool>();
    }

    /// <summary>
    /// Places a hook at given position
    /// </summary>
    /// <param name="ctx"></param>
    public void PlaceHook(IHookable ctx)
    {
        //Check if timer is up
        if (hookPlaceTimer > Time.time)
            return;

        //Reset timer
        hookPlaceTimer = Time.time + hookPlaceDelay;

        //Get available hook
        GameObject instance = objectPool.GetInstance();

        ctx.HookAdded(instance.transform);
    }

    public void DisposeHook(VoidEvent ctx)
    {
        //Remove oldest hook
        GameObject instance = objectPool.GetOldestInstance();
    }
}
