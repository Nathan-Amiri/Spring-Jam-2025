using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    [SerializeField] private Player player;

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.CompareTag("Terrain"))
        {
            player.isGrounded = true;
            player.hasJump = true;
        }
        else if (col.CompareTag("ItemTerrain"))
            player.hasJump = true; // Don't set player to isGrounded because items don't set deathwarp position!
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Terrain"))
        {
            player.isGrounded = false;
            player.hasJump = false;
        }
        else if (col.CompareTag("ItemTerrain"))
            player.hasJump = false;
    }
}