using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public GameObject speechBubblePrefab;
    public string[] dialogueLines;

    private bool playerInRange = false;
    private GameObject activeBubble;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.Return))
        {
            if (activeBubble == null)
            {
                Debug.Log("Start dialogue");

                Vector3 spawnPos = transform.position + Vector3.up * 2f;
                activeBubble = Instantiate(speechBubblePrefab, spawnPos, Quaternion.identity);
                activeBubble.transform.SetParent(transform);

                DialogueBubble bubble = activeBubble.GetComponent<DialogueBubble>();
                bubble.StartTyping(dialogueLines, () =>
                {
                    activeBubble = null; // Reset reference once dialogue ends
                });
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Transform root = other.transform.root;

        if (root.CompareTag("Player"))
        {
            // Debug.Log("Player entered NPC range");
            playerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        Transform root = other.transform.root;

        if (root.CompareTag("Player"))
        {
            playerInRange = false;

            if (activeBubble != null)
            {
                DialogueBubble bubble = activeBubble.GetComponent<DialogueBubble>();
                bubble.StopDialogue();
                activeBubble = null;
            }
        }
    }
}
