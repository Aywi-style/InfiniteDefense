using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMB : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        
    }

    private void OnTriggerStay(Collider other)
    {
        other.attachedRigidbody.AddForce(Vector3.up * 20f);
    }

    private void OnTriggerExit(Collider other)
    {
        
    }
}
