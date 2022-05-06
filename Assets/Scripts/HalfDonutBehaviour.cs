using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfDonutBehaviour : MonoBehaviour
{
    private enum Side { Left = -1, Right = 1 };
    [SerializeField]
    Side side;

    [SerializeField]
    private float timeRetracted, timeDeployed, deploymentSpeed, retractionSpeed;

    private float retractedPosition, deployedPosition;
    private new Rigidbody rigidbody;
    private System.Action NextMove;

    private void Awake()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    void Start()
    {
        retractedPosition = (int)side * (PlayerBehaviour.SwerveLimit + 2.35f);
        deployedPosition = retractedPosition - (int)side * 11.92f;
        Retract();
    }
    private void Retract()
    {
        NextMove = delegate { Deploy(); };
        StartCoroutine(Move(retractedPosition, retractionSpeed, timeRetracted));
    }
    private void Deploy()
    {
        NextMove = delegate { Retract(); };
        StartCoroutine(Move(deployedPosition, deploymentSpeed, timeDeployed));
    }

    private float direction;
    private Vector3 translation;
    IEnumerator Move(float target, float speed, float waitTime)
    {
        direction = Mathf.Sign(target - rigidbody.position.x);
        translation = Time.fixedDeltaTime * direction * speed * Vector3.right;

        while (direction * (rigidbody.position + translation).x < direction * target)
        {
            rigidbody.MovePosition(rigidbody.position + translation);
            yield return null;
        }
        rigidbody.MovePosition(new Vector3(target, rigidbody.position.y, rigidbody.position.z));

        yield return new WaitForSeconds(waitTime);
        NextMove();
    }
    

}
