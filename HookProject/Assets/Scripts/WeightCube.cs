using UnityEngine;

public class WeightCube : MonoBehaviour, IInteractable
{

    /// <summary>
    /// Pickup the cube
    /// </summary>
    public void Interact() { }
}

public interface IInteractable
{
    public void Interact();
}
