using UnityEngine;

[CreateAssetMenu(menuName = "Events/Hook Event Channel")]
public class HookEventChannel : GenericEventChannel<HookEvent> { }

[System.Serializable]
public struct HookEvent
{
    public Vector3 Position;
    public Transform Object;

    public HookEvent(Vector3 pos, ref Transform obj)
    {
        Position = pos;
        Object = obj;
    }
}