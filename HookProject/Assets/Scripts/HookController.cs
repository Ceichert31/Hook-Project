using DG.Tweening;
using UnityEngine;

public class HookController : MonoBehaviour
{
    [Header("DoTween Settings")]
    [SerializeField]
    private float hookPlaceDuration = 2f;
    [SerializeField]
    private float hookPlaceDelay = 1.3f;
    [SerializeField]
    private float cameraShakeDuration = 0.3f;
    [SerializeField]
    private float cameraShakeStrength = 0.3f;

    [SerializeField] 
    private Ease easeMode;

    [SerializeField] 
    private Transform cameraHolder;

    private HookPool objectPool;

    private GameObject hookObject;

    private float hookPlaceTimer;

    private void Start()
    {
        objectPool = GetComponent<HookPool>();
    }

    /// <summary>
    /// Places a hook at given position
    /// </summary>
    /// <param name="ctx"></param>
    public void PlaceHook(GameObject obj, Vector3 hitPoint)
    {
        //Check if timer is up
        if (hookPlaceTimer > Time.time) return;

        //Reset timer
        hookPlaceTimer = Time.time + hookPlaceDelay;

        //Get available hook
        GameObject instance = objectPool.GetInstance();

        //Unparent
        instance.transform.parent = obj.transform;

        //instance.GetComponent<HookInstance>().AttachedToObject();

        //Move
        instance.transform.DOMove(hitPoint, hookPlaceDuration).SetEase(Ease.InElastic).OnComplete(Shake);
    }

    public void DisposeHook(VoidEvent ctx)
    {
        //Remove oldest hook
        GameObject instance = objectPool.GetOldestInstance();

        //Lerp back to holder
        instance.transform.DOMove(transform.position, hookPlaceDuration).SetEase(Ease.InElastic).OnComplete(Shake);

        //Eventually restructure and move into HookPool
        //instance.SetActive(false);
    }

    /// <summary>
    /// Shakes the camera
    /// </summary>
    private void Shake()
    {
        //Camera shake
        cameraHolder.DOShakePosition(cameraShakeDuration, cameraShakeStrength, 10, 90, false, true).SetEase(easeMode);
    }
}
