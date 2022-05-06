using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField]
    private GameObject boy;
    [SerializeField]
    private float smoothTime = 0.5f, lookAhead = 20;
    [SerializeField]
    private Vector3 offset;
    private Vector3 velocity;

    private void Awake()
    {
        velocity = Vector3.zero;
    }
    private void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, boy.transform.position + offset, ref velocity, smoothTime);

        transform.LookAt(boy.transform.position + Vector3.forward * lookAhead);
    }
}
