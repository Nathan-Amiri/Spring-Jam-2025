using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [SerializeField] private Transform destinationReference;

    public float moveSpeed;

    private Vector3 start;
    private Vector3 destination;
    private bool reverse;

    public Rigidbody2D rb;

    private void Start()
    {
        start = transform.position;
        destination = destinationReference.position;

        rb.velocity = (destination - transform.position).normalized * moveSpeed;
    }
}