using System;
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

    private void Start()
    {
        DisableSledHook();
    }

    public void RaiseSledHook()
    {
        DOTween.CompleteAll();
        CancelInvoke(nameof(LowerSledHookAnimation));
        sledHook.gameObject.SetActive(true);
        //DoTween hook upwards when something is interactable
        sledHook.DOLocalMove(heldPosition.localPosition, moveTime);
    }

    public void LowerSledHook()
    {
        DOTween.CompleteAll();
        
        Invoke(nameof(LowerSledHookAnimation), lowerSledHookTime);
    }

    private void LowerSledHookAnimation()
    {
        sledHook.DOLocalMove(unequippedPosition.localPosition, moveTime).OnComplete(DisableSledHook);
    }

    private void DisableSledHook()
    {
        sledHook.gameObject.SetActive(false);
    }
}
