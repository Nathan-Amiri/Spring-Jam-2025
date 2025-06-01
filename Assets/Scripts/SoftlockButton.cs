using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoftlockButton : MonoBehaviour
{
    public SpriteRenderer sr;

    public Sprite defaultButton;
    public Sprite buttonPress;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            sr.sprite = buttonPress;
            col.GetComponent<Player>().PressButton();
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
            sr.sprite = defaultButton;
    }
}