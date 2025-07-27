using System;
using System.Collections.Generic;
using CodeBase.Grid;
using DG.Tweening;
using UnityEngine;

public class GridObjectMover : MonoBehaviour
{
    [SerializeField] private GridInitializer gridInitializer;
    [SerializeField] private MatchChecker matchChecker;
    [SerializeField] private MatchMerger matchMerger;
   
    private GridAnimator gridAnimator = new GridAnimator();
   public void MoveAllObjectsUp(Action onComplete)
{
    bool moved = false;

    for (int y = gridInitializer.GetHeight() - 2; y >= 0; y--)
    {
        for (int x = 0; x < gridInitializer.GetWidth(); x++)
        {
            var cell = gridInitializer.grid[x, y];
            if (!cell.IsEmpty)
            {
                Vector2Int fromPos = new Vector2Int(x, y);
                Vector2Int toPos = new Vector2Int(x, y + 1);

                if (IsWithinGrid(toPos) && gridInitializer.grid[toPos.x, toPos.y].IsEmpty)
                {
                    if (MoveObject(fromPos, Direction.Up))
                        moved = true;
                }
            }
        }
    }
    if (!moved)
    {
        onComplete?.Invoke();
        return;
    }
    List<List<GridCell>> allMatches = new();
    for (int x = 0; x < gridInitializer.GetWidth(); x++)
    {
        for (int y = 0; y < gridInitializer.GetHeight(); y++)
        {
            if (!gridInitializer.grid[x, y].IsEmpty)
            {
                var matches = matchChecker.CheckMatches(new Vector2Int(x, y));
                if (matches.Count >= 2)
                    allMatches.Add(matches);
            }
        }
    }
    if (allMatches.Count > 0)
    {
        int completed = 0;

        foreach (var match in allMatches)
        {
            matchMerger.Merge(match, 0.5f, () =>
            {
                completed++;
                if (completed == allMatches.Count)
                {
                    CheckLoseAndComplete(onComplete);
                }
            });
        }
    }
    else
    {
        CheckLoseAndComplete(onComplete);
    }
}
private void CheckLoseAndComplete(Action onComplete)
{
    for (int x = 0; x < gridInitializer.GetWidth(); x++)
    {
        if (!gridInitializer.grid[x, gridInitializer.GetHeight() - 1].IsEmpty)
        {
            GameEvents.LoseGame();
            return;
        }
    }
    onComplete?.Invoke();
}
    public bool MoveObject(Vector2Int fromPos, Direction dir)
    {
        Vector2Int toPos = GetNeighborPosition(fromPos, dir);
        if (!IsWithinGrid(fromPos) || !IsWithinGrid(toPos)) return false;

        var fromCell = gridInitializer.grid[fromPos.x, fromPos.y];
        var toCell = gridInitializer.grid[toPos.x, toPos.y];

        if (fromCell.IsEmpty || !toCell.IsEmpty) return false;

        GameObject obj = fromCell.content;
        fromCell.Clear();
        toCell.Fill(obj);

        gridAnimator.MoveAnimation(obj,GetCellCenter(toPos.x, toPos.y),0.5f);
        return true;
    }
    public Vector2Int FindObjectPosition(GameObject obj)
    {
        for (int x = 0; x < gridInitializer.GetWidth(); x++)
        for (int y = 0; y < gridInitializer.GetHeight(); y++)
            if (gridInitializer.grid[x, y].content == obj)
                return new Vector2Int(x, y);
        return new Vector2Int(-1, -1);
    }
    public Direction GetDirectionFromVector(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            return dir.x > 0 ? Direction.Right : Direction.Left;
        else
            return dir.y > 0 ? Direction.Down : Direction.Up;
    }
    public Vector2Int GetNeighborPosition(Vector2Int pos, Direction dir)
    {
        return dir switch
        {
            Direction.Up => pos + Vector2Int.up,
            Direction.Down => pos + Vector2Int.down,
            Direction.Left => pos + Vector2Int.left,
            Direction.Right => pos + Vector2Int.right,
            _ => pos
        };
    }
    public bool IsWithinGrid(Vector2Int pos) =>
        pos.x >= 0 && pos.x < gridInitializer.GetWidth() && pos.y >= 0 && pos.y < gridInitializer.GetHeight();

    private Vector3 GetCellCenter(int x, int y)
    {
        int invertedY = gridInitializer.GetHeight() - 1 - y;
        return new Vector3(
            transform.position.x + x * gridInitializer.GetCellSize() + gridInitializer.GetCellSize() / 2 + gridInitializer.GetOffset().x,
            transform.position.y + invertedY * gridInitializer.GetCellSize() + gridInitializer.GetCellSize() / 2 + gridInitializer.GetOffset().y,
            0
        );
    }
}