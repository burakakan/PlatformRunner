using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private Touch touch;

    private float touchLastPosition, mouseLastPosition;

    [SerializeField]
    private float sensitivity = 1f/10f;

    public static event Action FingerDown = delegate { };
    public static event Action<float> Swerve = delegate { };
    public static event Action<Vector2> SwerveFree = delegate { };
    public static event Action FingerUp = delegate { };

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                touchLastPosition = touch.position.x;
                FingerDown();
                Swerve(0);
                SwerveFree(touch.position);
            }

            else if (touch.phase == TouchPhase.Ended)
                FingerUp();

            else
            {
                Swerve(Mathf.Clamp((touch.position.x - touchLastPosition) * sensitivity, -1, 1));
                touchLastPosition = touch.position.x;
                SwerveFree(touch.position);
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                mouseLastPosition = Input.mousePosition.x;
                FingerDown();
                Swerve(0);
                SwerveFree(Input.mousePosition);
            }

            else if (Input.GetMouseButtonUp(0))
                FingerUp();

            else if (Input.GetMouseButton(0))
            {
                Swerve(Mathf.Clamp((Input.mousePosition.x - mouseLastPosition) * sensitivity, -1, 1));
                mouseLastPosition = Input.mousePosition.x;
                SwerveFree(Input.mousePosition);
            }
        }

    }
}
