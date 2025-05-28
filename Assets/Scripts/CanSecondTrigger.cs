using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanSecondTrigger : MonoBehaviour
{
    [SerializeField] private Item can;
    private void OnTriggerEnter2D(Collider2D col)
    {
        can.OnEnterCanSecondTrigger(col, true);
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        can.OnEnterCanSecondTrigger(col, false);
    }
}