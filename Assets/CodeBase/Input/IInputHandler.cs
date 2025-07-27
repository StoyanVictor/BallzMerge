using UnityEngine;

public interface IInputHandler
{
    bool IsAiming { get; }
    bool IsReleased { get; }
    Vector2 GetDirection(Vector3 fromPosition);
    void Update();
}