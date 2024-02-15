using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelteObject : MonoBehaviour
{
    public float delay = 5f; // Time in seconds before the object is deleted

    void Start()
    {
        // Invoke the DestroyObject method after the specified delay
        Invoke("DestroyObject", delay);
    }

    void DestroyObject()
    {
        // Destroy the object this script is attached to
        Destroy(gameObject);
    }
}
