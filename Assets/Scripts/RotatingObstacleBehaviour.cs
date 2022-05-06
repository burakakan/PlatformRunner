using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingObstacleBehaviour : MonoBehaviour
{
    [SerializeField]
    private float rotationalSpeed;

    public float RotationalSpeed { get; private set; }

    [SerializeField]
    private Transform rotator;

    private Vector3 rotation;
    private new Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
        RotationalSpeed = rotationalSpeed;
        rotation = new Vector3(0, rotationalSpeed, 0);
    }

    void Start()
    {
        rigidbody.centerOfMass = rotator.position - transform.position;
    }

    void FixedUpdate()
    {
        rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler(Time.fixedDeltaTime * rotation));
    }
}
