using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [SerializeField] private Transform destinationReference;
    [SerializeField] private SpriteRenderer destinationSR;

    public float moveSpeed;
    public float pauseDuration;

    private Vector3 start;
    private Vector3 destination;
    private int state; // 0 = pause at start, 1 = move to destination, 2 = pause at destination, 3 = move to start

    public Rigidbody2D rb;

    private void Start()
    {
        start = transform.position;
        destination = destinationReference.position;

        destinationSR.enabled = false;

        StartCoroutine(HaltAtStart());
    }
    private void Update()
    {
        if (state == 1 && Vector2.Distance(transform.position, destination) < .2f)
        {
            state = 2;
            StartCoroutine(HaltAtDestination());
        }
        if (state == 3 && Vector2.Distance(transform.position, start) < .2f)
        {
            state = 0;
            StartCoroutine(HaltAtStart());
        }
    }
    private IEnumerator HaltAtStart()
    {
        rb.velocity = Vector2.zero;
        transform.position = start;

        yield return new WaitForSeconds(pauseDuration);

        state = 1;
        rb.velocity = (destination - transform.position).normalized * moveSpeed;
    }
    private IEnumerator HaltAtDestination()
    {
        rb.velocity = Vector2.zero;
        transform.position = destination;

        yield return new WaitForSeconds(pauseDuration);

        state = 3;
        rb.velocity = (start - transform.position).normalized * moveSpeed;
    }
}