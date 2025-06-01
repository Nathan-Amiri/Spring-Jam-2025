using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoldItemIcon : MonoBehaviour
{
    public SpriteRenderer iconSR;

    public List<Sprite> icons = new(); // 0 = mushroom, 1 = cookie, 2 = spinach, 3 = corn dog, 4 = broccoli, 5 = carrot, 6 = spaghetti, 7 = can

    public Vector2 npcOffset;

    public void ToggleIcon(bool on, Item item)
    {
        iconSR.enabled = on;
        iconSR.flipX = false;
        iconSR.flipY = false;

        if (!on)
            return;

        switch (item.itemName)
        {
            case "Mushroom":
                iconSR.sprite = icons[0];
                transform.localPosition = new Vector2(0, 1.7f);
                transform.localScale = new Vector2(.8f, .8f);
                break;

            case "Cookie":
                iconSR.sprite = icons[1];
                transform.localPosition = new Vector2(0, 1.7f);
                transform.localScale = new Vector2(.8f, .8f);
                break;

            case "Spinach":
                iconSR.sprite = icons[2];
                transform.localPosition = new Vector2(0, 1.7f);
                transform.localScale = new Vector2(.8f, .8f);
                iconSR.flipX = true;
                iconSR.flipY = true;
                break;

            case "Corn Dog":
                iconSR.sprite = icons[3];
                transform.localPosition = new Vector2(0, 1.8f);
                transform.localScale = new Vector2(.8f, .8f);
                break;

            case "Broccoli":
                iconSR.sprite = icons[4];
                transform.localPosition = new Vector2(0, 2.5f);
                transform.localScale = new Vector2(.8f, .8f);
                break;

            case "Carrot":
                iconSR.sprite = icons[5];
                transform.localPosition = new Vector2(0, 1.3f);
                transform.localScale = new Vector2(.6f, .6f);
                break;

            case "Spaghetti":
                iconSR.sprite = icons[6];
                transform.localPosition = new Vector2(.2f, 1.7f);
                transform.localScale = new Vector2(.8f, .8f);
                break;

            case "Can":
                iconSR.sprite = icons[7];
                transform.localPosition = new Vector2(-.2f, 1.9f);
                transform.localScale = new Vector2(.8f, .8f);
                break;
        }

        transform.localPosition += (Vector3)npcOffset;
    }
}