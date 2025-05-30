using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public Rigidbody2D rb;

    private void Start()
    {
        rb.velocity = Vector2.right * .5f;
    }
}