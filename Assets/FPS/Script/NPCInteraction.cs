using UnityEngine;
using UnityEngine.InputSystem;

public class NPCInteraction : MonoBehaviour
{
   // public string promptMessage = "Press E to interact";
    public AudioClip interactionClip;
    private AudioSource audioSource;
    private bool playerInRange = false;
    public AudioClip explainClip;
    private Animator animator;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (playerInRange && Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("E key pressed for interaction.");
            ShowChoices();
            Interact();
        }
    }

    private void ShowChoices()
    {
        UIManager.Instance.ShowChoices(
            onExplain: () =>
            {
                Debug.Log("Player chose to explain the temple.");
                if (explainClip != null)
                {
                    audioSource.clip = explainClip;
                    audioSource.Play();
                }
            },
            onSkip: () =>
            {
                Debug.Log("Player skipped the dialogue.");
            }
        );
    }
    private void Interact()
    {
        // Play sound or trigger dialogue
        if (interactionClip != null)
        {
            audioSource.clip = interactionClip;
            audioSource.Play();
        }
        Debug.Log("NPC Interaction Triggered!");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered NPC range.");

            if(interactionClip != null)
            {
                audioSource.clip = interactionClip;
                audioSource.Play();
            }

            UIManager.Instance.ShowChoices(
                onExplain: () => PlayExplanation(),
                onSkip: () => SkipExplanation()
                );
            // playerInRange = true;
            //UIManager.Instance.ShowPrompt(promptMessage); // call your UI manager
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player left NPC zone → hiding choices");
            UIManager.Instance.HideChoices();
            StopExplanation();
        }
    }

    private void PlayExplanation()
    {
        Debug.Log("Player chose to explain the temple.");

        if (explainClip != null)
        {
            audioSource.clip = explainClip;
            audioSource.Play();

            // Start animation
            if (animator != null)
                animator.SetBool("isTalking", true);

            // Stop after clip finishes
            Invoke(nameof(StopExplanation), explainClip.length);
        }
    }

    private void SkipExplanation()
    {
        Debug.Log("Player skipped explanation.");
        StopExplanation();
    }

    private void StopExplanation()
    {
        // Stop audio
        if (audioSource.isPlaying)
            audioSource.Stop();

        // Reset animation
        if (animator != null)
            animator.SetBool("isTalking", false);
    }
}
    