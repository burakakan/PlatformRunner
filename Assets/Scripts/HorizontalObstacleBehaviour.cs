using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HorizontalObstacleBehaviour : MonoBehaviour
{
    [SerializeField]
    private float linearSpeed, rotationalSpeed, limit;
    private int direction;
    
    //private Quaternion rotation;
    private Vector3 translation, rotation;
    private new Rigidbody rigidbody;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        direction = -1;
        rotation = new Vector3(0, rotationalSpeed, 0);
        translation = new Vector3(linearSpeed, 0, 0);
    }

    private Vector3 position;
    void FixedUpdate()
    {
        position = rigidbody.position;
        if (position.x > limit || position.x < -limit)
            direction *= -1;

        rigidbody.MovePosition(position + direction * Time.fixedDeltaTime * translation);
        rigidbody.MoveRotation(rigidbody.rotation * Quaternion.Euler(-direction * Time.fixedDeltaTime * rotation));
    }
   
}
