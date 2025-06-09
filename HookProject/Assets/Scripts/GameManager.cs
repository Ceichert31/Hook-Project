using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;

    public Transform PlayerTransform { get { return playerTransform; } }

    public static GameManager Instance;

    private void Awake()
    {
        Instance = this;
    }
}
