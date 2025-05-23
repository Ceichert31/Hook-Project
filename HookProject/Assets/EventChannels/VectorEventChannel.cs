using UnityEngine;
[CreateAssetMenu(menuName = "Events/Vector Event Channel")]
public class VectorEventChannel : GenericEventChannel<VectorEvent> {}

[System.Serializable]
public struct VectorEvent
{
    public Vector3 Value;
    public VectorEvent(Vector3 value)
    {
        Value = value;
    }
}