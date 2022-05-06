using System.Collections;
using UnityEngine;
using static UnityEngine.Mathf;

public class PlayerBehaviour : MonoBehaviour
{
    [SerializeField]
    private float maxPace = 10f, maxSwerveSpeed = 15f, swerveLimit = 7.5f, acceleration = 20f, deceleration = 20f, rotationCorrectionRate = 45f, reboundCoefficient = 3f, takeOffCoefficient = 2f, respawnWait = 2;
    private float pace = 0;
    public static float SwerveLimit { get; private set; }

    private Animator animator;
    private int blend;

    private new Rigidbody rigidbody;

    private void Awake()
    {
        SwerveLimit = swerveLimit;
        animator = gameObject.GetComponent<Animator>();
        blend = Animator.StringToHash("Blend");
    }

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        Spawn();
    }
    private void StartMoving()
    {
        StopAllCoroutines();
        StartCoroutine(Transition(acceleration));
    }
    private void StopMoving()
    {
        StopAllCoroutines();
        StartCoroutine(Transition(-1 * deceleration));
    }
    IEnumerator Transition(float acceleration)
    {
        rigidbody.angularVelocity = Vector3.zero;
        for (; (acceleration < 0 && pace > 0) || (acceleration > 0 && pace < maxPace); pace += Time.deltaTime * acceleration)   //do the pace transition to maxPace when accelerating or to 0 when decelerating
        {
            //set the animation blend according to pace ratio. 0: idle, 1: running at full speed
            animator.SetFloat(blend, pace / maxPace);

            if (acceleration < 0) Move(0);  //keep moving while decelerating despite FingerUp()
            yield return null;
        }
        pace = acceleration > 0 ? maxPace : 0;
        animator.SetFloat(blend, pace / maxPace);
    }
    private Vector3 position, velocity;
    private float deflection;
    void Move(float swerveInput)
    {
        velocity = (rigidbody.position - position) / Time.fixedDeltaTime;
        position = rigidbody.position;

        //move in x axis within the swerve limits and go forward at current pace
        rigidbody.MovePosition(new Vector3(Clamp(position.x + swerveInput * maxSwerveSpeed * Time.fixedDeltaTime, -SwerveLimit, SwerveLimit), position.y, position.z + pace * Time.fixedDeltaTime));

        //calculate the angle between the motion direction and z axis
        deflection = Atan((swerveInput * maxSwerveSpeed + 0.01f) / (pace + 0.01f)) / PI * 180;

        //gradually rotate the character towards the motion direction
        transform.eulerAngles += Sign((transform.eulerAngles.y + 360 - deflection) % 360 - 180) * rotationCorrectionRate * Time.fixedDeltaTime * Vector3.up;
    }

    Vector3 contactPoint, relativeVelocity, rebound;
    private void OnCollisionEnter(Collision collision)
    {
        if (gameObject.layer == 8 || collision.gameObject.layer != 6) return;    //if the character has hit an obstacle before (if it's in layer 8) or the object that's hit is not in the obstacle layer let the collision happen as usual

        //make the character rebound from obstacles with custom forces
                
        contactPoint = collision.GetContact(0).point;

        //calculate the character's velocity relative to the contact point on the obstacle
        //get the velocity
        relativeVelocity = velocity;
        if (collision.rigidbody)    //can't get the point velocity if the object doesn't have a rigidbody
            //subtract the contact point's velocity
            relativeVelocity -= collision.rigidbody.GetPointVelocity(contactPoint);

        //rebound vector is in the reverse direction to the relative velocity and has a magnitude proportional to the rebound coefficient
        rebound = -1 * reboundCoefficient * relativeVelocity;

        //apply the rebound force at the contact point instead of the center of mass for better rebounding effect
        rigidbody.AddForceAtPosition(rebound, contactPoint, ForceMode.VelocityChange);

        //additional velocity in y direction proportional to the rebound velocity for take off effect
        rigidbody.AddForce(takeOffCoefficient * rebound.magnitude * Vector3.up, ForceMode.VelocityChange);

        OutOfPlay();
    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.layer == 9)
        {
            //if the player leaves the rotating platform at a lower point than -0.5, it has fallen off the platform
            if (rigidbody.position.y < -0.5)
            {
                OutOfPlay();
                return;
            }
            //restore the swerve limit when back on the stationary platform
            SwerveLimit = swerveLimit;
        }
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == 9)
            //ignore the swerve limit when the player is on the rotating platform
            SwerveLimit = Infinity;
    }
    private void OutOfPlay()
    {
        rigidbody.constraints = RigidbodyConstraints.None;
        animator.applyRootMotion = false;

        //disable controls
        PlayerInput.FingerDown -= StartMoving;
        PlayerInput.OnSwipe -= Move;
        PlayerInput.FingerUp -= StopMoving;

        gameObject.layer = 8;   //move to the layer of collided characters

        StartCoroutine(RespawnCountdown());
    }
    IEnumerator RespawnCountdown()
    {
        yield return new WaitForSeconds(respawnWait);
        Spawn();
    }
    private void Spawn()
    {
        //reset position, velocity and rotation
        transform.position = Vector3.zero;
        rigidbody.velocity = Vector3.zero;
        rigidbody.angularVelocity = Vector3.zero;
        transform.rotation = Quaternion.Euler(Vector3.zero);
        pace = 0;

        //reset the animator
        animator.SetFloat(blend, 0);
        animator.applyRootMotion = true;
        
        //restore constraints
        rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        
        //enable controls
        PlayerInput.FingerDown += StartMoving;
        PlayerInput.OnSwipe += Move;
        PlayerInput.FingerUp += StopMoving;

        //get the player on characters layer
        gameObject.layer = 3;

        //restore the swerve limit
        SwerveLimit = swerveLimit;
    }
}
