using System;

public class GameEvents
{
    public static event Action OnBallUsed;
    public static event Action<int> OnMoveChanged;
    public static event Action OnGameLose;
    public static event Action OnBallHit;
    public static event Action OnBlocsDestroyed;

    public static void BallUsed()
    {
        OnBallUsed?.Invoke();
    }
    public static void BlockDestroyed()
    {
        OnBlocsDestroyed?.Invoke();
    }
    public static void BallHits()
    {
        OnBallHit?.Invoke();
    }
    public static void LoseGame()
    {
        OnGameLose?.Invoke();
    }

    public static void MoveChanged(int newValue)
    {
        OnMoveChanged?.Invoke(newValue);
    }
}
