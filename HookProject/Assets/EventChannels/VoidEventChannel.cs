using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventChannels
{
    [CreateAssetMenu(menuName = "Events/Void Event Channel")]
    public class VoidEventChannel : GenericEventChannel<VoidEvent> {}
    [System.Serializable]
    public struct VoidEvent {}
}
