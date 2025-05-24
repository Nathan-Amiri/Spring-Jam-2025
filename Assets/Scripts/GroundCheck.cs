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
            StartCoroutine(CoyoteTime());
        }
    }
    private IEnumerator CoyoteTime()
    {
        yield return new WaitForSeconds(player.coyoteTime);
        player.hasJump = false;
        // It's safe to remove after a duration no matter what since jump is added every physics frame if the player is grounded
    }
}