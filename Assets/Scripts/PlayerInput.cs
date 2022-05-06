using System;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    private Touch touch;

    private float touchLastPosition, mouseLastPosition;

    [SerializeField]
    private float sensitivity = 1f/10f;

    public static event Action FingerDown = delegate { };
    public static event Action<float> OnSwipe = delegate { };
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
                OnSwipe(0);
            }

            else if (touch.phase == TouchPhase.Ended)
                FingerUp();

            else
            {
                OnSwipe(Mathf.Clamp((touch.position.x - touchLastPosition) * sensitivity, -1, 1));
                touchLastPosition = touch.position.x;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                mouseLastPosition = Input.mousePosition.x;
                FingerDown();
                OnSwipe(0);
            }

            else if (Input.GetMouseButtonUp(0))
                FingerUp();

            else if (Input.GetMouseButton(0))
            {
                OnSwipe(Mathf.Clamp((Input.mousePosition.x - mouseLastPosition) * sensitivity, -1, 1));
                mouseLastPosition = Input.mousePosition.x;
            }
        }

    }
}
