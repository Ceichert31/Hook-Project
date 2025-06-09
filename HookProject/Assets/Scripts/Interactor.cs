using UnityEngine;
using NaughtyAttributes;
using System;
public class Interactor : MonoBehaviour
{
    [Header("Event Channel References")]
    [SerializeField]
    private VectorEventChannel hookPositionChannel;
    private VectorEvent vectorEvent;


    [Header("Hook Settings")]
    [Layer]
    [SerializeField]
    private int hookLayer;

    [SerializeField]
    private LayerMask hookMask;

    [SerializeField]
    private Transform spherecastOrigin;

    [SerializeField]
    private float spherecastRange = 5.0f;
    [SerializeField]
    private float spherecastRadius = 1.5f;

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
            if (Physics.SphereCast(spherecastOrigin.position, spherecastRadius, spherecastOrigin.forward, out RaycastHit hit, spherecastRange, hookMask))
            {
                if (hit.transform.gameObject.TryGetComponent(out MoveableObject instance))
                {
                    //Activate and place hook
                    vectorEvent.Value = hit.point;

                    //Send point over and sign to place hook
                    hookPositionChannel.CallEvent(vectorEvent);
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
