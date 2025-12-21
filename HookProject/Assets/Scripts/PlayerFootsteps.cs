using UnityEngine;

public class PlayerFootsteps : MonoBehaviour
{
    [SerializeField]
    private float walkingSpeed = 3;

    [SerializeField]
    private float footstepInterval = 2;

    [SerializeField]
    private AudioPitcherSO snowFootstepAudio;

    float audioTimer = 0.0f;

    private InputController inputController;

    private AudioSource source;

    private void Awake()
    {
        inputController = GetComponentInParent<InputController>();
        source = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (inputController.IsMoving && inputController.IsGrounded)
        {
            audioTimer += walkingSpeed * Time.deltaTime;

            if (audioTimer > footstepInterval)
            {
                //Reset timer and play sound effect
                audioTimer = 0.0f;
                snowFootstepAudio.Play(source);
            }
        }
        else
        {
            audioTimer = 0.0f;
        }
    }
}
