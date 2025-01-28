using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinueRotation : MonoBehaviour
{
    private float rotationSpeed = 20f;
    void Update()
    {
        // Rotate the object continuously
        transform.Rotate(0f, 0f, -rotationSpeed * Time.deltaTime);
    }
}
