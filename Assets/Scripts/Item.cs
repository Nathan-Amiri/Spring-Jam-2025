using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class Item : MonoBehaviour
{
    // PREFAB REFERENCE:
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private List<Collider2D> myCols; // All colliders. Used only by Item
    public Collider2D closestCollider; // The collider used to determine which item is closest to Player. Used only by Player
    public List<Collider2D> noClipColliders; // The non-trigger colliders that can noclip through walls/hazards. Used only by Can

    [SerializeField] private SpriteRenderer pickupIconSR;

    [SerializeField] private FixedJoint2D cornDogJoint;

    public string itemName; // Read by Player

    // CONSTANT:
    private readonly float mushroomBounceStrength = 45;
    private readonly float mushroomCookieBounceStrength = 15;
    private readonly float mushroomCeleryBounceStrength = 30;
    private readonly float cookieThrowSpeed = 8;
    private readonly float cornDogThrowSpeed = 13;
    private readonly float cornDogEmbedDistance = .75f; // The distance between the center of the corn dog and the end of the corn dog (the start of the stick)

    // DYNAMIC:
    private RigidbodyConstraints2D defaultConstraints;

    // Items are set uninteractable on pickup, since colliders won't turn off in time to prevent some interactions (e.g. falling onto mushroom)
    private bool uninteractable;

    private Item cornDogAttachmentItem; // The item Corn Dog is attached to, if any

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

        // Corn Dog stick into terrain/hazard
        if (itemName == "Corn Dog" && (col.CompareTag("Hazard") || col.CompareTag("Terrain") || col.CompareTag("ItemTerrain")))
        {
            // MyCols[1] is the stick trigger collider
            transform.position = col.bounds.ClosestPoint(myCols[1].transform.position) - (transform.right * cornDogEmbedDistance);

            cornDogJoint.enabled = true;
            cornDogJoint.connectedBody = col.attachedRigidbody;

            // The Item script will always be on the collider object or its parent
            if (col.TryGetComponent(out Item attachmentItem))
                cornDogAttachmentItem = attachmentItem;
            else if (col.transform.parent != null && col.transform.parent.TryGetComponent(out Item attachmentParentItem))
                cornDogAttachmentItem = attachmentParentItem;
        }

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
    private void OnTriggerStay2D(Collider2D col)
    {
        // Mushroom bounce celery
        if (col.transform.parent != null && col.transform.parent.name == "Celery" && itemName == "Mushroom")
            col.attachedRigidbody.AddForceAtPosition(Vector2.up * mushroomCeleryBounceStrength, transform.position + new Vector3(0, .5f), ForceMode2D.Impulse);
    }

    private void Update()
    {
        if (cornDogAttachmentItem != null && cornDogAttachmentItem.uninteractable)
        {
            cornDogAttachmentItem = null;
            cornDogJoint.enabled = false;
        }
    }

    public void Pickup()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        sr.enabled = false;
        foreach (Collider2D col in myCols)
            col.enabled = false;

        uninteractable = true;

        if (itemName == "Corn Dog")
        {
            cornDogAttachmentItem = null;
            cornDogJoint.enabled = false;
        }
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

            case "Celery":
                transform.position = player.transform.position + new Vector3(1 * xDirection, 4.5f);
                transform.rotation = Quaternion.Euler(0, facingLeft ? 180 : 0, 85);
                break;

            case "Spinach":
                transform.position = player.transform.position + new Vector3(0, 1);
                break;

            case "Corn Dog":
                transform.position = player.transform.position + new Vector3(.5f * xDirection, 0);
                transform.rotation = Quaternion.Euler(0, facingLeft ? 180 : 0, 0);
                rb.velocity = new Vector2(xDirection * cornDogThrowSpeed, 0);
                break;

            case "Can":
                transform.position = player.transform.position + new Vector3(2 * xDirection, .75f);
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

    public void OnEnterCanSecondTrigger(Collider2D col, bool enter) // Only the Can will run this method
    {
        // The easiest way to handle multiple triggers on a single rigidbody is to have a second script that can only detect one trigger
        // This method is called whenever a collider enters the Can's NoClip trigger

        List<Collider2D> cols = new();

        if (col.TryGetComponent(out Player _))
        {
            cols.Add(col);
        }
        else if (col.TryGetComponent(out Item item))
        {
            cols = item.noClipColliders;
        }
        else
            return;

        if (cols.Count == 0) // Not all items have noClipColliders (not all can pass through can)
            return;

        int layer1 = LayerMask.NameToLayer("NoClipable");
        // HazardCollision layer acts like both NoClipable and PlayerPassThrough (works with can but doesn't normally collide with player)
        int layer2 = LayerMask.NameToLayer("HazardCollision");
        LayerMask mask = (1 << layer1) | (1 << layer2);

        foreach (Collider2D c in cols)
        {
            if (enter)
                c.excludeLayers = mask;
            else
                c.excludeLayers = default;
        }
    }
}