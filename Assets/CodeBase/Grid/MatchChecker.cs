using System.Collections.Generic;
using UnityEngine;

public class MatchChecker : MonoBehaviour
{
    [SerializeField] private GridInitializer gridInitializer;

    public List<GridCell> CheckMatches(Vector2Int startPos)
    {
        var obj = gridInitializer.grid[startPos.x, startPos.y].content;
        if (obj == null) return new List<GridCell>();

        string targetType = obj.GetComponent<GridObject>().Id;
        return FindMatches(startPos, targetType);
    }
    private List<GridCell> FindMatches(Vector2Int startPos, string Id)
    {
        List<GridCell> matches = new List<GridCell>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(startPos);

        while (queue.Count > 0)
        {
            var pos = queue.Dequeue();
            if (!IsWithinGrid(pos)) continue;

            var cell = gridInitializer.grid[pos.x, pos.y];
            if (cell.IsEmpty || matches.Contains(cell)) continue;
            if (cell.content.GetComponent<GridObject>().Id != Id) continue;

            matches.Add(cell);

            queue.Enqueue(pos + Vector2Int.up);
            queue.Enqueue(pos + Vector2Int.down);
            queue.Enqueue(pos + Vector2Int.left);
            queue.Enqueue(pos + Vector2Int.right);
        }
        return matches;
    }

    private bool IsWithinGrid(Vector2Int pos) =>
        pos.x >= 0 && pos.x < gridInitializer.GetWidth() && pos.y >= 0 && pos.y < gridInitializer.GetHeight();
}