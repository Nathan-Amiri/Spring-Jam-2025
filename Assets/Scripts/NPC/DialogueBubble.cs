using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueBubble : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    public AudioClip[] blipSounds;
    private AudioSource audioSource;

    // Default speaking speed
    public float typingSpeed = 0.03f;

    // Optional callback to notify when dialogue finishes
    private System.Action onDialogueComplete;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private Coroutine typingRoutine;

    public void StartTyping(string[] lines, System.Action onComplete = null)
    {
        onDialogueComplete = onComplete;
        typingRoutine = StartCoroutine(TypeLines(lines));
    }

    public void StopDialogue()
    {
        if (typingRoutine != null)
            StopCoroutine(typingRoutine);

        if (onDialogueComplete != null)
            onDialogueComplete.Invoke();

        Destroy(gameObject);
    }

    private IEnumerator TypeLines(string[] lines)
    {
        foreach (string line in lines)
        {
            textUI.text = "";

            int charCount = 0;

            foreach (char c in line)
            {
                textUI.text += c;

                if (!char.IsWhiteSpace(c))
                {
                    charCount++;

                    // Plays every other character to reduce audio clutter
                    if (charCount % 2 == 0 && blipSounds.Length > 0)
                    {
                        AudioClip blip = blipSounds[Random.Range(0, blipSounds.Length)];
                        audioSource.PlayOneShot(blip);
                    }
                }

                yield return new WaitForSeconds(typingSpeed);
            }

            // Wait for Enter before moving to next line
            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
        }

        // Call completion callback if present
        if (onDialogueComplete != null)
            onDialogueComplete.Invoke();

        // Remove speech bubble when done
        Destroy(gameObject);
    }
}
