using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornDogGroundTrigger : MonoBehaviour
{
    public Item cornDog;
    private void OnTriggerEnter2D(Collider2D col)
    {
        cornDog.CornDogStickIntoStuff(col, true);
    }
}