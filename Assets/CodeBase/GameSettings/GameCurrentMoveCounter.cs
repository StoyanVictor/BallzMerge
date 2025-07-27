using UnityEngine;

[CreateAssetMenu(fileName = "GameCurrentMoveCounter", menuName = "Game/CurrentMoveCounter")]
public class GameCurrentMoveCounter : ScriptableObject
{
    public int currentMove;

    public void ResetCounter()
    {
        currentMove = 0;
    }
}