using UnityEngine;

public class PlayerFollow : MonoBehaviour
{
    [SerializeField]
    Transform player;

    [SerializeField]
    float cameraOffsetY = 25f;

    private void Update()
    {
        transform.position = new(
            player.position.x,
            player.position.y + cameraOffsetY,
            player.position.z
        );
    }
}
