using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] private Player player;

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Terrain"))
            player.isGrounded = true;
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Terrain"))
        {
            player.isGrounded = false;
            player.hasJump = false;
        }
    }
}