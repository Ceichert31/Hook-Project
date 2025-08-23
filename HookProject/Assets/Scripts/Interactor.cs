using System;
using EventChannels;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

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
    private LayerMask hookMask;

    [SerializeField]
    private Transform raycastOrigin;

    [SerializeField]
    private float raycastRange = 5.0f;

    private bool canHook;
    private bool isSledHookRaised;

    private HookController hookController;
    private SledHookVisualController sledHookController;
    
    //Need:
    //Channel for adding hooks
    //Channel for removing hooks

    //On trigger stay with interactable objects fire raycast to check if object is interactable

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
                hookMask
            )
        )
        {
            //Guard clause
            if (other.gameObject.layer != hookLayer)
                return;

            if (hit.transform.gameObject.TryGetComponent(out IHookable instance))
            {
                //Player interacts state
                if (canHook)
                {
                    canHook = false;
                    sledHookController.SetHookedState(true);
                    hookController.PlaceHook(instance);
                }
            }
            //Raise sled hook when looking at interactable
            if (isSledHookRaised) return;
            
            isSledHookRaised = true;
            sledHookController.RaiseSledHook();
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
    /// Player input to allow hook input
    /// </summary>
    /// <param name="ctx"></param>
    public void HookInput(VoidEvent ctx)
    {
        canHook = true;
        Invoke(nameof(ResetCanHook), 0.05f);
    }

    private void ResetCanHook() => canHook = false;
}
