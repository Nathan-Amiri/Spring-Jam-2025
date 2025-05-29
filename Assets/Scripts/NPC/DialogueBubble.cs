using System.Collections;
using TMPro;
using UnityEngine;

public class DialogueBubble : MonoBehaviour
{
    public TextMeshProUGUI textUI;
    public AudioClip[] blipSounds;
    private AudioSource audioSource;

    public float typingSpeed = 0.03f;

    private System.Action onDialogueComplete;
    private Coroutine typingRoutine;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

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
            bool skipping = false;

            yield return new WaitForSeconds(0.1f); // short buffer to avoid accidental skip

            for (int i = 0; i < line.Length; i++)
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    skipping = true;
                    break;
                }

                textUI.text += line[i];

                if (!char.IsWhiteSpace(line[i]) && (charCount % 2 == 0) && blipSounds.Length > 0)
                {
                    AudioClip blip = blipSounds[Random.Range(0, blipSounds.Length)];
                    audioSource.PlayOneShot(blip);
                }

                charCount++;
                yield return new WaitForSeconds(typingSpeed);
            }

            if (skipping)
            {
                textUI.text = line;
                yield return new WaitForSeconds(0.1f);
            }

            yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
        }

        if (onDialogueComplete != null)
            onDialogueComplete.Invoke();

        Destroy(gameObject);
    }
}
