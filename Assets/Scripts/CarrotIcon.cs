using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrotIcon : MonoBehaviour
{
    private void Update()
    {
        transform.rotation = Quaternion.Euler(0, 0, transform.parent.rotation.z);
    }
}