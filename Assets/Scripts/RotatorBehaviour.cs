using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatorBehaviour : MonoBehaviour
{
    [SerializeField]
    private GameObject stick;

    private float rotationalSpeed;

    void Start()
    {
        rotationalSpeed = stick.GetComponent<RotatingObstacleBehaviour>().RotationalSpeed;
    }

    void Update()
    {
        transform.Rotate(Time.deltaTime * rotationalSpeed * Vector3.up);
    }
}
