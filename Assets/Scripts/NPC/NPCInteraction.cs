using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public string desiredItemName;

    public GameObject speechBubblePrefab;

    public HoldItemIcon holdItemIcon;

    public SpriteRenderer talkIcon;

    public string[] bothEmptyDialogueLines;     // both empty
    public string[] npcItemDialogueLines;    // npc item player empty
    public string[] wrongItemDialogueLines;    // player has wrong item, NPC empty
    public string[] rightItemDialogueLines;    // player has right item, NPC empty
    public string[] bothFullDialogueLines;    // npc and player both full

    private bool playerInRange = false;
    private GameObject activeBubble;

    public Player player;

    private Item heldItem;

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }

    void Update()
    {
        talkIcon.enabled = playerInRange && activeBubble == null;

        if (playerInRange && Input.GetKeyDown(KeyCode.P))
        {
            if (activeBubble == null)
            {
                string[] dialogueLines = null;
                if (heldItem == null && player.heldItem == null)
                    dialogueLines = bothEmptyDialogueLines;
                else if (heldItem != null && player.heldItem == null)
                {
                    dialogueLines = npcItemDialogueLines;
                    ExchangeItem(false);
                }
                else if (heldItem == null && player.heldItem.itemName != desiredItemName)
                    dialogueLines = wrongItemDialogueLines;
                else if (heldItem == null && player.heldItem.itemName == desiredItemName)
                {
                    dialogueLines = rightItemDialogueLines;
                    ExchangeItem(true);
                }
                else if (heldItem != null && player.heldItem != null)
                    dialogueLines = bothFullDialogueLines;
                else
                    Debug.LogError("Whoops, Nathan must've forgot something!");

                Vector3 spawnPos = transform.position + Vector3.up * 3.5f + Vector3.right * 3.5f;
                activeBubble = Instantiate(speechBubblePrefab, spawnPos, Quaternion.identity);
                activeBubble.transform.SetParent(transform);

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

    private void ExchangeItem(bool gainItem)
    {
        if (gainItem)
        {
            heldItem = player.heldItem;
            holdItemIcon.ToggleIcon(true, heldItem);

            player.ExchangeItem(false, null);
        }
        else
        {
            player.ExchangeItem(true, heldItem);

            heldItem = null;
            holdItemIcon.ToggleIcon(false, null);
        }
    }
}
