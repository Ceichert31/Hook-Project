using DG.Tweening;
using UnityEngine;

public class SledHookVisualController : MonoBehaviour
{
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
    
    private void Start()
    {
        sledHookRenderer = sledHook.GetComponent<MeshRenderer>();
        
        DisableSledHook();
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
    public void RaiseSledHook()
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
    public void LowerSledHook()
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
        sledHookRenderer.enabled = true;
    }
}
