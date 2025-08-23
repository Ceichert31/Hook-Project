using DG.Tweening;
using UnityEngine;

/// <summary>
/// Controls the visuals of the sled hook, and changes them based on the interaction state
/// </summary>
public class SledHookVisualController : MonoBehaviour
{
    [Header("Visual Controller Settings")]
    [SerializeField]
    private Transform sledHook;

    [SerializeField]
    private Transform heldPosition;

    [SerializeField]
    private Transform unequippedPosition;

    [SerializeField]
    private float moveTime = 0.5f;

    [SerializeField]
    private float lowerSledHookTime = 0.5f;

    private MeshRenderer sledHookRenderer;

    private bool isHooked;
    private bool isHookRaised;
    
    private void Start()
    {
        sledHookRenderer = sledHook.GetComponent<MeshRenderer>();
    }

    /// <summary>
    /// Updates the position of the sled hook's based on the state 
    /// </summary>
    /// <param name="state">The current state of interaction</param>
    public void UpdateInteractionState(InteractionState state)
    {
        switch (state)
        {
            case InteractionState.None:
                if (!isHookRaised) return;
                isHookRaised = false;
                LowerSledHook();
                break;
            
            case InteractionState.Hookable:
                if (isHookRaised) return;
                isHookRaised = true;
                RaiseSledHook();
                break;
        }
    }

    /// <summary>
    /// Sets the current state of the hook
    /// </summary>
    /// <param name="hookedState">Whether the sled is connected to the player</param>
    /// <remarks>
    /// If true, the hook will not be animated
    /// </remarks>
    public void SetHookedState(bool hookedState) => isHooked = hookedState;
    
    /// <summary>
    /// Animates the sled hook upwards and enables it's renderer
    /// </summary>
    private void RaiseSledHook()
    {
        if (isHooked) return;
        
        DOTween.CompleteAll();
        CancelInvoke(nameof(LowerSledHookAnimation));
        sledHookRenderer.enabled = true;
        //DoTween hook upwards when something is interactable
        sledHook.DOLocalMove(heldPosition.localPosition, moveTime);
    }

    /// <summary>
    /// Sets timer to lower sled hook
    /// </summary>
    private void LowerSledHook()
    {
        if (isHooked) return;
        
        DOTween.CompleteAll();
        Invoke(nameof(LowerSledHookAnimation), lowerSledHookTime);
    }

    /// <summary>
    /// Animates sled hook downward and disabled mesh renderer
    /// </summary>
    private void LowerSledHookAnimation()
    {
        sledHook.DOLocalMove(unequippedPosition.localPosition, moveTime).OnComplete(DisableSledHook);
    }

    private void DisableSledHook()
    {
        sledHookRenderer.enabled = false;
    }
}
