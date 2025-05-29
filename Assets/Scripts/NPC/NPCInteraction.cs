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
                //Debug.Log("Start dialogue");

                Vector3 spawnPos = transform.position + Vector3.up * 3f;
                activeBubble = Instantiate(speechBubblePrefab, spawnPos, Quaternion.identity);
                activeBubble.transform.SetParent(transform);

                Player player = FindObjectOfType<Player>();
                player.SetStunned(true);

                DialogueBubble bubble = activeBubble.GetComponent<DialogueBubble>();
                bubble.StartTyping(dialogueLines, () =>
                {
                    player.SetStunned(false);
                    activeBubble = null;
                });
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Transform root = other.transform.root;

        if (root.CompareTag("Player"))
        {
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
