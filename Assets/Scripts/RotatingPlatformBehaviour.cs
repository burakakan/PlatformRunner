using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingPlatformBehaviour : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed = 15;

    private new Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //transform.Rotate(Time.fixedDeltaTime * rotationSpeed * Vector3.forward);

        rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler(Time.fixedDeltaTime * rotationSpeed * Vector3.forward));
    }
}
