using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [SerializeField] private Transform destination;

    public float moveSpeed = 5;

    private Vector3 startPosition;
    private float fraction = 0;

    private bool reverse;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (Vector2.Distance(transform.position, destination.position) < .1f)
        {
            reverse = true;
        }
        else if (Vector2.Distance(transform.position, startPosition) < .1f)
        {
            reverse = false;
        }

        if (reverse)
        {
            fraction += Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(startPosition, destination.position, fraction);
        }
        else
        {
            fraction -= Time.deltaTime * moveSpeed;
            transform.position = Vector3.Lerp(destination.position, startPosition, fraction);
        }
    }
}