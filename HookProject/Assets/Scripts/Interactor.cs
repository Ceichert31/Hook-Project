using EventChannels;
using NaughtyAttributes;
using UnityEngine;

public enum InteractionState
{
    None,
    Hookable,
    Interactable
}

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

    [SerializeField]
    private LayerMask interactableLayers;

    [SerializeField]
    private Transform raycastOrigin;

    [SerializeField]
    private float raycastRange = 5.0f;

    private HookController hookController;
    private SledHookVisualController sledHookController;
    
    //Input flags
    private bool canInteract;
    private bool canRemoveHook;

    private InteractionState previousState;
    private InteractionState currentState;
    
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
                interactableLayers
            )
        )
        {
            IsSurfaceHookable(hit);
            IsSurfaceInteractable(hit);
        }
        else
        {
            currentState = InteractionState.None;
        }

        //Update state information if state has changed
        if (currentState == previousState) return;
        
        sledHookController.UpdateInteractionState(currentState);
        previousState = currentState;
    }

    /// <summary>
    /// Checks if a ray cast surface contains an <see cref="IHookable"/> interface
    /// </summary>
    /// <param name="hit">The collision point of the raycast</param>
    private void IsSurfaceHookable(RaycastHit hit)
    {
        if (hit.transform.gameObject.layer != hookLayer) return;

        if (!hit.transform.gameObject.TryGetComponent(out IHookable instance)) return;
        
        currentState = InteractionState.Hookable;
            
        //Player left-click input
        if (canInteract)
        {
            canInteract = false;
                
            sledHookController.SetHookedState(true);
            hookController.PlaceHook(instance);
                
            currentState = InteractionState.None;
        }
            
        //Player right-click input
        if (!canRemoveHook) return;
        canRemoveHook = false;
        sledHookController.SetHookedState(false);
        hookController.RemoveHook(instance);
    }

    /// <summary>
    /// Checks whether a surface is interactable and executes the interact logic on the object
    /// </summary>
    /// <param name="hit">The collision point of the raycast</param>
    private void IsSurfaceInteractable(RaycastHit hit)
    {
        return;
        //Execute interact logic on object
        //NOTE INCOMPLETE
        if (hit.transform.gameObject.layer != interactableLayers.value) return;

        if (!hit.transform.gameObject.TryGetComponent(out IInteractable instance)) return;
        if (!canInteract) return;
        
        instance.Interact();
    }
    
    /// <summary>
    /// Signal from input controller to allow hook placement
    /// </summary>
    /// <param name="ctx">Void context</param>
    public void PlaceHook_Input(VoidEvent ctx)
    {
        canInteract = true;
        Invoke(nameof(ResetCanHook), RESET_INPUT_TIME);
    }

    /// <summary>
    /// Signal from input controller to allow hook removal
    /// </summary>
    /// <param name="ctx">Void context</param>
    public void RemoveHook_Input(VoidEvent ctx)
    {
        canRemoveHook = true;
        Invoke(nameof(ResetCanHook), RESET_INPUT_TIME);
    }

    /// <summary>
    /// Resets player input flags
    /// </summary>
    private void ResetCanHook()
    {
        canInteract = false;
        canRemoveHook = false;
    } 
}
