using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // PREFAB REFERENCE:
    public Rigidbody2D rb; // Read by Item (Mushroom)
    [SerializeField] private PolygonCollider2D myCol;

    // SCENE REFERENCE:
    [SerializeField] private Camera mainCamera;

    // CONSTANT:
    private readonly float gravityScale = 3.5f;
    private readonly float moveSpeed = 8;
    private readonly float jumpForce = 15;
    private readonly float fallMultiplier = 3; // Fastfall
    private readonly float lowJumpMultiplier = 10; // Dynamic jump
    [NonSerialized] public readonly float coyoteTime = .1f; // Read by GroundCheck

    private readonly float deathWarpDuration = .15f;

    private readonly float deathY = -50; // Needs to be a value below the map, to prevent softlocks in case the player manages to get out of bounds

    private readonly float spinachMaxFallSpeed = 3;

    // DYNAMIC:
    private float moveInput;
    private bool jumpInputDown;
    private bool jumpInput;

    private bool isStunned;

    private Coroutine deathWarpRoutine;

    private bool facingLeft; // Read by Item

    private readonly List<Item> itemsInPickupRange = new();
    private bool itemPickupInput;
    private Item heldItem;

    private bool dynamicJumpOff;

    // Set by GroundCheck:
    [NonSerialized] public bool isGrounded;
    [NonSerialized] public Vector2 lastGroundedPosition;
    [NonSerialized] public bool hasJump;

    public void Start()
    {
        if (deathWarpRoutine != null)
        {
            StopCoroutine(deathWarpRoutine);
            myCol.enabled = true;
        }
    }
    private void Update()
    {
        rb.gravityScale = isStunned ? 0 : gravityScale;

        if (transform.position.y < deathY)
            Die();

        if (rb.velocity.y <= 0)
            dynamicJumpOff = false;



        if (isStunned)
            return;



        PickupItem();

        if (isGrounded)
            lastGroundedPosition = transform.position;

        moveInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
            jumpInputDown = true;

        jumpInput = Input.GetButton("Jump");

        if (moveInput > 0)
            facingLeft = false;
        else if (moveInput < 0)
            facingLeft = true;



        // I know this is a weird way to code this but I think it'll be better at preventing bugs than a more clean method
        if (Input.GetButtonDown("Item"))
        {
            if (heldItem != null)
                DropItem();
            else
                itemPickupInput = true;
        }
        if (!Input.GetButton("Item"))
            itemPickupInput = false;
    }

    private void FixedUpdate()
    {
        if (isStunned)
            return;

        if (heldItem != null && heldItem.itemName == "Spinach" && rb.velocity.y < 0)
        {
            float ySpeed = rb.velocity.y;
            ySpeed = Mathf.Clamp(ySpeed, -spinachMaxFallSpeed, 0);
            rb.velocity = new Vector2(rb.velocity.x, ySpeed);
        }

        // Snappy horizontal movement:
        // (This movement method will prevent the player from slowing completely in a frictionless environment. To prevent this,
        // this rigidbody's linear drag is set to .01)
        float desiredVelocity = moveInput * moveSpeed;
        float velocityChange = desiredVelocity - rb.velocity.x;
        float acceleration = velocityChange / .05f;
        float force = rb.mass * acceleration;
        rb.AddForce(new(force, 0));

        // Fastfall
        if (rb.velocity.y < 0)
            // Subtract fall and lowjump multipliers by 1 to more accurately represent the multiplier (fallmultiplier = 2 means fastfall will be x2)
            rb.velocity += (fallMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up;
        // Dynamic jump (see TurnOffDynamicJump for more info)
        else if (rb.velocity.y > 0 && (!jumpInput || dynamicJumpOff))
            rb.velocity += (lowJumpMultiplier - 1) * Physics2D.gravity.y * Time.deltaTime * Vector2.up;

        if (isGrounded)
            hasJump = true;

        if (jumpInputDown)
        {
            if (!hasJump)
                jumpInputDown = false;
            else
            {
                jumpInputDown = false;
                hasJump = false;

                rb.velocity = new(rb.velocity.x, jumpForce);
            }
        }
    }
    private void LateUpdate()
    {
        mainCamera.transform.position = new(transform.position.x, transform.position.y, -10);
    }

    public void Die()
    {
        myCol.enabled = false;

        deathWarpRoutine = StartCoroutine(DeathWarp(deathWarpDuration));

        isStunned = true;

        float warpSpeed = Vector2.Distance(lastGroundedPosition, transform.position) / deathWarpDuration;
        rb.velocity = warpSpeed * ((Vector3)lastGroundedPosition - transform.position).normalized;
    }
    private IEnumerator DeathWarp(float duration)
    {
        yield return new WaitForSeconds(duration);

        rb.velocity = Vector3.zero;
        transform.position = lastGroundedPosition + new Vector2(0, .5f);

        isStunned = false;

        myCol.enabled = true;

        hasJump = true; // Softlock prevention
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        // 1. Add item if it's in range
        if (col.TryGetComponent(out Item itemInRange))
            itemsInPickupRange.Add(itemInRange);
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        // 2. Remove item if it leaves range and turn off its icon
        if (col.TryGetComponent(out Item itemInRange))
        {
            itemInRange.TogglePickupIcon(false);
            itemsInPickupRange.Remove(itemInRange);
        }
    }
    private void PickupItem() // Run in Update
    {
        if (itemsInPickupRange.Count == 0)
            return;

        Item closestItemInRange = null;

        // 3. Turn on icon for the closest item in range and turn off icon for any other items in range
        if (itemsInPickupRange.Count == 1)
            closestItemInRange = itemsInPickupRange[0];
        else
        {
            float distanceToClosestItem = 999;
            foreach (Item item in itemsInPickupRange)
            {
                Vector2 closestPointInItemColliderBounds = item.triggerCol.bounds.ClosestPoint(transform.position);
                float distanceToItem = Vector2.Distance(closestPointInItemColliderBounds, transform.position);

                if (distanceToItem < distanceToClosestItem)
                {
                    closestItemInRange = item;
                    distanceToClosestItem = distanceToItem;
                }
            }
        }

        foreach (Item item in itemsInPickupRange)
            item.TogglePickupIcon(item == closestItemInRange);

        // 4. Pick up closest item if input and not holding item
        if (!itemPickupInput || heldItem != null)
            return;

        heldItem = closestItemInRange;
        closestItemInRange.Pickup();
    }
    private void DropItem()
    {
        heldItem.Drop(this, facingLeft);
        heldItem = null;
    }

    public void TurnOffDynamicJump()
    {
        // Called by Items that provide an upward boost that shouldn't be affected by dynamic jump
        // While dynamic jump is off, player is permanently weighty as if the jump button wasn't held
        // Turns back on once the player is no longer moving upwards
        dynamicJumpOff = true;
    }
}