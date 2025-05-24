using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // PREFAB REFERENCE:
    [SerializeField] private string itemName;

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!col.CompareTag("Player"))
            return;

        Player player = col.GetComponent<Player>();

        switch (itemName)
        {
            case "Mushroom":
                if (player.rb.velocity.y >= 0)
                    break;

                // Set velocity instead of adding force so that current fall speed doesn't affect bounce height
                player.rb.velocity = new(player.rb.velocity.x, 35);
                player.hasJump = true;
                break;
        }
    }
}