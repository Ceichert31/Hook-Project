using EventChannels;
using NaughtyAttributes;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    [Header("Event Channel References")]
    [SerializeField]
    private HookEventChannel hookPositionChannel;
    private HookEvent hookEvent;

    [Header("Hook Settings")]
    [Layer]
    [SerializeField]
    private int hookLayer;  
    
    [Layer]
    [SerializeField]
    private int interactLayer;

    [SerializeField]
    private Transform raycastOrigin;

    [SerializeField]
    private float raycastRange = 5.0f;

    private HookController hookController;
    private SledHookVisualController sledHookController;
    
    private bool canPlaceHook;
    private bool canRemoveHook;
    private bool isSledHookRaised;
    
    private const float RESET_INPUT_TIME = 0.05f;
    private void Start()
    {
        hookController = GetComponent<HookController>();
        sledHookController = GetComponent<SledHookVisualController>();
    }

    private void OnTriggerStay(Collider other)
    {
        //Fire raycast to check if object in view is hookable
        if (
            Physics.Raycast(
                raycastOrigin.position,
                raycastOrigin.forward,
                out RaycastHit hit,
                raycastRange,
                hookLayer
            )
        )
        {
            IsSurfaceHookable(hit);
            IsSurfaceInteractable(hit);
        }
        else
        {
            //Lower hook and prevent interaction
            if (!isSledHookRaised) return;
            
            isSledHookRaised = false;
            sledHookController.LowerSledHook();
        }
    }

    /// <summary>
    /// Checks if a ray cast surface contains an <see cref="IHookable"/> interface
    /// </summary>
    /// <param name="hit">The collision point of the raycast</param>
    private void IsSurfaceHookable(RaycastHit hit)
    {
        if (hit.transform.gameObject.layer != hookLayer) return;
        
        if (hit.transform.gameObject.TryGetComponent(out IHookable instance))
        {
            //Player left-click input
            if (canPlaceHook)
            {
                canPlaceHook = false;
                sledHookController.SetHookedState(true);
                hookController.PlaceHook(instance);
            }
            
            //Player right-click input
            if (!canRemoveHook) return;
            canRemoveHook = false;
            sledHookController.SetHookedState(false);
            //Remove hook
        }
        
        //Raise sled hook when looking at interactable
        if (isSledHookRaised) return;
        
        isSledHookRaised = true;
        sledHookController.RaiseSledHook();
    }

    /// <summary>
    /// Checks whether a surface is interactable and executes the interact logic on the object
    /// </summary>
    /// <param name="hit">The collision point of the raycast</param>
    private void IsSurfaceInteractable(RaycastHit hit)
    {
        //Execute interact logic on object
        if (hit.transform.gameObject.layer != interactLayer) return;

        if (hit.transform.gameObject.TryGetComponent(out IInteractable instance))
        {
            instance.Interact();
        }
    }
    
    /// <summary>
    /// Player input to allow hook input
    /// </summary>
    /// <param name="ctx"></param>
    public void HookInput(VoidEvent ctx)
    {
        canPlaceHook = true;
        Invoke(nameof(ResetCanHook), RESET_INPUT_TIME);
    }

    public void RemoveHook_Input(VoidEvent ctx)
    {
        canRemoveHook = true;
        Invoke(nameof(ResetCanHook), RESET_INPUT_TIME);
    }

    private void ResetCanHook()
    {
        canPlaceHook = false;
        canRemoveHook = false;
    } 
}
