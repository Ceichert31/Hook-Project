using DG.Tweening;
using UnityEngine;

public class CameraShakeManager : MonoBehaviour
{
    public static CameraShakeManager Instance;

    [SerializeField]
    private Transform cameraHolder;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Shakes the players camera
    /// </summary>
    /// <param name="duration">How long to shake</param>
    /// <param name="intensity">How far the shaking should range from</param>
    /// <param name="ease">The easing function</param>
    public void ShakeCamera(float duration, float intensity, Ease ease) =>
        cameraHolder.DOShakePosition(duration, intensity).SetEase(ease);
}
