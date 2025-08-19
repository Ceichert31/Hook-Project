using DG.Tweening;
using EventChannels;
using UnityEngine;

public class HookController : MonoBehaviour
{
    [Header("DoTween Settings")]
    [SerializeField]
    private float hookPlaceDuration = 2f;

    [SerializeField]
    private float hookPlaceDelay = 1.3f;

    [SerializeField]
    private Transform sledHook;
    
    private float _hookPlaceTimer;

    /// <summary>
    /// Places a hook at given position
    /// </summary>
    /// <param name="ctx"></param>
    public void PlaceHook(IHookable ctx)
    {
        //Check if timer is up
        if (_hookPlaceTimer > Time.time)
            return;

        //Reset timer
        _hookPlaceTimer = Time.time + hookPlaceDelay;

        //Get available hook
        //GameObject instance = objectPool.GetInstance();

        ctx.HookAdded(sledHook.transform);
    }

    public void DisposeHook(VoidEvent ctx)
    {
        //Remove oldest hook
        //GameObject instance = objectPool.GetOldestInstance();
    }
}
