using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // PREFAB REFERENCE:
    public Rigidbody2D rb; // Read by Item (Mushroom)
    [SerializeField] private PolygonCollider2D col;

    // SCENE REFERENCE:
    [SerializeField] private Camera mainCamera;

    // CONSTANT:
    private readonly float gravityScale = 3.5f;
    private readonly float moveSpeed = 8;
    private readonly float jumpForce = 15;
    private readonly float fallMultiplier = 3; // Fastfall
    private readonly float lowJumpMultiplier = 10; // Dynamic jump
    private readonly float jumpBuffer = .08f;
    [NonSerialized] public readonly float coyoteTime = .1f; // Read by GroundCheck

    private readonly float deathWarpDuration = .15f;

    private readonly float deathY = -50; // Needs to be a value below the map, to prevent softlocks in case the player manages to get out of bounds

    // DYNAMIC:
    private float moveInput;
    private bool jumpInputDown;
    private bool jumpInput;

    private bool isStunned;

    private Coroutine deathWarpRoutine;

    [NonSerialized] public bool facingLeft; // Read by Item

    private bool itemPickupReady;
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
            col.enabled = true;
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
                itemPickupReady = true;
        }
        if (!Input.GetButton("Item"))
            itemPickupReady = false;
    }

    private void FixedUpdate()
    {
        if (isStunned)
            return;

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
                StartCoroutine(JumpBuffer());
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

    private IEnumerator JumpBuffer()
    {
        yield return new WaitForSeconds(jumpBuffer);
        jumpInputDown = false;
    }

    public void Die()
    {
        col.enabled = false;

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

        col.enabled = true;

        hasJump = true; // Softlock prevention
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (!itemPickupReady || heldItem != null || !col.CompareTag("Item"))
            return;

        Item item = col.GetComponent<Item>();

        heldItem = item;
        item.Pickup();
    }
    private void DropItem()
    {
        heldItem.Drop(this);
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