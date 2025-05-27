using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookieIcon : MonoBehaviour
{
    private void Update()
    {
        transform.SetPositionAndRotation(transform.parent.position + (Vector3.up * 1), Quaternion.Euler(0, 0, transform.parent.rotation.z));
    }
}