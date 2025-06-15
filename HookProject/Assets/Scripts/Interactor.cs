using UnityEngine;
using NaughtyAttributes;
using System;
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
    [SerializeField]
    private float raycastRadius = 1.5f;

    private bool canHook;

    //Need:
    //Channel for adding hooks 
    //Channel for removing hooks


    //On trigger stay with interactable objects fire raycast to check if object is interactable

    private void OnTriggerStay(Collider other)
    {
        //Guard clause
        if (other.gameObject.layer != hookLayer) return;

        //Check for player input here
        if (canHook)
        {
            canHook = false;

            //Fire raycast to check if object in view is hookable
            if (Physics.Raycast(raycastOrigin.position, raycastOrigin.forward, out RaycastHit hit, raycastRange, hookMask))
            {
                if (hit.transform.gameObject.TryGetComponent(out IHookable instance))
                {
                    //Activate and place hook
                    hookEvent.Position = hit.point;
                    hookEvent.Object = hit.transform;

                    //Send point over and sign to place hook
                    hookPositionChannel.CallEvent(hookEvent);

                    instance.HookAdded();
                }
            }
        }
    }

    /// <summary>
    /// Player input to allow hook input
    /// </summary>
    /// <param name="ctx"></param>
    public void HookInput(VoidEvent ctx) => canHook = true;
}