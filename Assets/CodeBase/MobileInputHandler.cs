using UnityEngine;

public class MobileInputHandler : IInputHandler
{
    private Vector2 startTouch;
    private bool released;

    public bool IsAiming { get; private set; }
    public bool IsReleased => released;

    public Vector2 GetDirection(Vector3 fromPosition)
    {
        Vector2 endTouch = Input.mousePosition;
        return (endTouch - startTouch).normalized;
    }

    public void Update()
    {
        released = false;

        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.phase == TouchPhase.Began)
            {
                startTouch = touch.position;
                IsAiming = true;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                IsAiming = false;
                released = true;
            }
        }
    }
}