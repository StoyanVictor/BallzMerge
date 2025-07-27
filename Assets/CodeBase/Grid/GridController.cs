using UnityEngine;

public class GridController : MonoBehaviour
{
    [SerializeField] private GridInitializer gridInitializer;
    [SerializeField] private GridSpawner gridSpawner;
    [SerializeField] private GridObjectMover gridObjectMover;
    [SerializeField] private MatchChecker matchChecker;
    [SerializeField] private MatchMerger matchMerger;
    [SerializeField] private BallPoolSpawner ballController;
    [SerializeField] private GameCurrentMoveCounter moveCounter;
    private void Start()
    {
        gridSpawner.SpawnInitialObjects(moveCounter.currentMove);
    }
    public void OnBallHit(GameObject hitObject, Vector2 hitDirection)
    {
        Vector2Int fromPos = gridObjectMover.FindObjectPosition(hitObject);
        if (fromPos.x == -1) return;

        Direction dir = gridObjectMover.GetDirectionFromVector(hitDirection);
        if (gridObjectMover.MoveObject(fromPos, dir))
        {
            Vector2Int toPos = gridObjectMover.GetNeighborPosition(fromPos, dir);
            var matches = matchChecker.CheckMatches(toPos);

            if (matches.Count >= 2)
            {
                matchMerger.Merge(matches,0.5f,null);
            }
        }
    }
    public void OnBallSettled()
    {
        gridObjectMover.MoveAllObjectsUp(() =>
        {
            gridSpawner.SpawnInitialObjects(moveCounter.currentMove);
        });
    }
}