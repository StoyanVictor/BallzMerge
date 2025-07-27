using UnityEngine;

public class PcInputHandler : IInputHandler
{
    public bool IsAiming => Input.GetMouseButton(0);
    public bool IsReleased => Input.GetMouseButtonUp(0);

    public Vector2 GetDirection(Vector3 fromPosition)
    {
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return (mouseWorld - (Vector2)fromPosition).normalized;
    }

    public void Update() { }
}