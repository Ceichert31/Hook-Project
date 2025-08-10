using DG.Tweening;
using UnityEngine;

public class SledHookVisualController : MonoBehaviour
{
    [SerializeField]
    private Transform sledHook;

    [SerializeField]
    private Vector3 heldPosition;

    [SerializeField]
    private Vector3 unequipedPosition;

    [SerializeField]
    private float moveTime = 0.5f;

    public void HoldSledHook()
    {
        DOTween.CompleteAll();
        //DoTween hook upwards when something is interactable
        //sledHook.DOLocalMove(heldPosition, moveTime);
    }

    public void LowerSledHook()
    {
        DOTween.CompleteAll();

        //sledHook.DOLocalMove(unequipedPosition, moveTime);
    }
}
