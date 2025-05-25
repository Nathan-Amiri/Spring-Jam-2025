using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // PREFAB REFERENCE:
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private List<Collider2D> cols;

    [SerializeField] private string itemName;

    // DYNAMIC:
    RigidbodyConstraints2D defaultConstraints;

    private void Awake()
    {
        defaultConstraints = rb.constraints;
    }

    private void OnTriggerEnter2D(Collider2D playerCol)
    {
        if (!playerCol.CompareTag("Player"))
            return;

        Player player = playerCol.GetComponent<Player>();

        switch (itemName)
        {
            case "Mushroom":
                if (player.rb.velocity.y >= 0)
                    break;

                // Set velocity instead of adding force so that current fall speed doesn't affect bounce height
                player.rb.velocity = new(player.rb.velocity.x, 45);
                break;
        }
    }

    public void Pickup()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        sr.enabled = false;
        foreach (Collider2D col in cols)
            col.enabled = false;
    }

    public void Drop(Player player)
    {
        rb.constraints = defaultConstraints;
        sr.enabled = true;
        foreach (Collider2D col in cols)
            col.enabled = true;

        int xDirection = player.facingLeft ? -1 : 1;

        switch (itemName)
        {
            default:
                transform.position = player.transform.position + new Vector3(.5f * xDirection, 0);
                break;
        }
    }
}