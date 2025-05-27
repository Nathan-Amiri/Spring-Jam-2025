using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    // PREFAB REFERENCE:
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private List<Collider2D> myCols; // All colliders, including trigger collider. Used only by Item
    public Collider2D triggerCol; // Just trigger collider. Used only by Player

    [SerializeField] private SpriteRenderer pickupIconSR;

    [SerializeField] private string itemName;

    // CONSTANT:
    private readonly float mushroomBounceStrength = 45;
    private readonly float mushroomCookieBounceStrength = 15;
    private readonly float cookieThrowSpeed = 8;

    // DYNAMIC:
    private RigidbodyConstraints2D defaultConstraints;

    // Items are set uninteractable on pickup, since colliders won't turn off in time to prevent some interactions (e.g. falling onto mushroom)
    private bool uninteractable;

    private void Awake()
    {
        defaultConstraints = rb.constraints;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // Mushroom bounce cookie/broccoli
        if (col.gameObject.name == "Broccoli" || col.gameObject.name == "Cookie")
            if (itemName == "Mushroom" && col.transform.position.y > transform.position.y + .5f)
                col.attachedRigidbody.velocity = new(col.attachedRigidbody.velocity.x, mushroomCookieBounceStrength);



        if (uninteractable || !col.CompareTag("Player"))
            return;

        Player player = col.GetComponent<Player>();

        switch (itemName)
        {
            case "Mushroom":
                if (player.rb.velocity.y >= 0)
                    break;

                // Set velocity instead of adding force so that current fall speed doesn't affect bounce height
                player.rb.velocity = new(player.rb.velocity.x, mushroomBounceStrength);
                player.TurnOffDynamicJump();
                break;
        }
    }

    public void Pickup()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        sr.enabled = false;
        foreach (Collider2D col in myCols)
            col.enabled = false;

        uninteractable = true;
    }

    public void Drop(Player player, bool facingLeft)
    {
        rb.constraints = defaultConstraints;
        sr.enabled = true;
        foreach (Collider2D col in myCols)
            col.enabled = true;

        int xDirection = facingLeft ? -1 : 1;

        switch (itemName)
        {
            case "Cookie":
                transform.position = player.transform.position + new Vector3(.5f * xDirection, 0);
                rb.velocity = new Vector2(xDirection * cookieThrowSpeed, 0);
                break;

            case "Broccoli":
                transform.position = player.transform.position + new Vector3(.5f * xDirection, 2);
                break;

            default:
                transform.position = player.transform.position + new Vector3(.5f * xDirection, 0);
                break;
        }

        uninteractable = false;
    }

    public void TogglePickupIcon(bool on)
    {
        pickupIconSR.enabled = on;
    }
}