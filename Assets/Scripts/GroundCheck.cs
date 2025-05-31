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

        if (col.CompareTag("Terrain") || col.CompareTag("ItemTerrain"))
            if (col.gameObject.name != "Cookie")
                player.SetMovingPlatform(col.transform);
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Terrain"))
        {
            player.isGrounded = false;
            player.hasJump = false;

            player.SetMovingPlatform(null);
        }
        else if (col.CompareTag("ItemTerrain"))
            player.hasJump = false;

        if (col.CompareTag("Terrain") || col.CompareTag("ItemTerrain"))
            if (col.gameObject.name != "Cookie")
                player.SetMovingPlatform(null);
    }
}