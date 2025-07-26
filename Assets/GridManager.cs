using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Grid Settings")]
    [SerializeField] private int width = 8;
    [SerializeField] private int height = 8;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Vector2 gridOffset = Vector2.zero;
    [SerializeField] private GameObject blockPrefab;
    [SerializeField] private Color gridColor = Color.white;
    [SerializeField] private int initialSpawnCount = 2;
    [SerializeField] private SpawnRulesSO rules;
    [SerializeField] private SpawnProbabilitySO rulesProbability;
    
    public int currentMove;
    private GridCell[,] grid;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitializeGrid();
        SpawnInitialObjects();
    }
    void InitializeGrid()
    {
        grid = new GridCell[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                grid[x, y] = new GridCell(x, y);
            }
        }
    }

    void SpawnInitialObjects()
    {
        int blocksToSpawn = rulesProbability.GetBlocksToSpawn(currentMove);
        Debug.LogError(blocksToSpawn);
        for (int i = 0; i < blocksToSpawn; i++)
        {
            int randomX = Random.Range(0, width);
            if (grid[randomX, 0].IsEmpty)
            {
                SpawnRandomObject(randomX, 0);
            }
        }
    }
    public void MoveAllObjectsUp()
    {
        bool moved = false;
        for (int y = height - 2; y >= 0; y--)
        {
            for (int x = 0; x < width; x++)
            {
                var cell = grid[x, y];
                if (!cell.IsEmpty)
                {
                    Vector2Int fromPos = new Vector2Int(x, y);
                    Vector2Int toPos = new Vector2Int(x, y + 1);

                    if (IsWithinGrid(toPos) && grid[toPos.x, toPos.y].IsEmpty)
                    {
                        if (MoveObject(fromPos, Direction.Up))
                        {
                            moved = true;
                            CheckMatches(toPos);
                        }
                    }
                }
            }
        }
        if (!moved)
        {
            Debug.LogWarning("No objects moved, so no spawn!");
        }

        if (moved) SpawnInitialObjects(); // ✅ Только один раз
    }


    public void SpawnRandomObject(int x, int y)
    {
        Block[] allowedBlocks = rules.GetAllowedNumbers(currentMove);
        Block chosenBlock = allowedBlocks[Random.Range(0, allowedBlocks.Length)];
        GameObject obj = Instantiate(blockPrefab, GetCellCenter(x, y), Quaternion.identity);
        obj.GetComponent<GridObject>().Init(chosenBlock.id,chosenBlock.color);
        grid[x, y].Fill(obj);
    }

    public void OnBallHit(GameObject hitObject, Vector2 hitDirection)
    {
        Vector2Int fromPos = FindObjectPosition(hitObject);
        if (fromPos.x == -1) return;

        Direction dir = GetDirectionFromVector(hitDirection);
        if (MoveObject(fromPos, dir))
        {
            Vector2Int toPos = GetNeighborPosition(fromPos, dir);
            CheckMatches(toPos);
            Debug.LogWarning("Hello");
        }
    }
    bool MoveObject(Vector2Int fromPos, Direction dir)
    {
        Vector2Int toPos = GetNeighborPosition(fromPos, dir);
        if (!IsWithinGrid(fromPos) || !IsWithinGrid(toPos)) return false;

        var fromCell = grid[fromPos.x, fromPos.y];
        var toCell = grid[toPos.x, toPos.y];

        if (fromCell.IsEmpty || !toCell.IsEmpty) return false;

        GameObject obj = fromCell.content;

        fromCell.Clear();
        toCell.Fill(obj);
        // 2. Создаем анимацию
        Sequence moveSequence = DOTween.Sequence();
    
        // Анимация движения к новой позиции
        moveSequence.Append(obj.transform.DOMove(
                GetCellCenter(toPos.x, toPos.y), 
                0.5f)
            .SetEase(Ease.OutQuad));
    
        // Анимация "встряски" (по желанию)
        moveSequence.Join(obj.transform.DOShakeScale(
                0.3f, 
                0.4f, 
                10, 
                90f, 
                true)
            .SetEase(Ease.OutElastic));
        
        return true;
    }
    public void CheckMatches(Vector2Int startPos)
    {
        var obj = grid[startPos.x, startPos.y].content;
        if (obj == null) return;

        string targetType = obj.GetComponent<GridObject>().Id;
        List<GridCell> matches = FindMatches(startPos, targetType);

        if (matches.Count >= 2) // Минимум 2 для матча
        {
            MergeAnimation(matches,0.5f);
            print($"Match found! Count: {matches.Count}");
        }
    }

    public void MergeAnimation(List<GridCell> matches, float duration)
    {
        List<GameObject> objs = new List<GameObject>();
        Vector3 centerPoint = Vector3.zero;

        foreach (var cell in matches)
        {
            if (cell.content != null)
            {
                objs.Add(cell.content);
                centerPoint += cell.content.transform.position;
            }
        }
        centerPoint /= objs.Count;

        foreach (var cell in matches)
        {
            GameObject obj = cell.content;
            if (obj == null) continue;

            cell.Clear();
            var collider = obj.GetComponent<Collider2D>();
            if (collider != null) collider.enabled = false;

            Sequence mergeSequence = DOTween.Sequence();
            mergeSequence.Join(obj.transform.DOMove(centerPoint, duration * 0.7f).SetEase(Ease.InQuad));
            mergeSequence.Join(obj.transform.DOScale(Vector3.zero, duration).SetEase(Ease.InBack));

            var renderer = obj.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                mergeSequence.Join(renderer.DOColor(Color.white * 1.5f, duration * 0.3f).SetLoops(2, LoopType.Yoyo));
            }

            mergeSequence.OnComplete(() => {
                if (obj != null) Destroy(obj);
            });
        }
    }

   List<GridCell> FindMatches(Vector2Int startPos, string Id)
    {
        List<GridCell> matches = new List<GridCell>();
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        queue.Enqueue(startPos);

        while (queue.Count > 0)
        {
            var pos = queue.Dequeue();
            if (!IsWithinGrid(pos)) continue;

            var cell = grid[pos.x, pos.y];
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
    Vector2Int FindObjectPosition(GameObject obj)
    {
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                if (grid[x, y].content == obj)
                    return new Vector2Int(x, y);
        return new Vector2Int(-1, -1);
    }

    Direction GetDirectionFromVector(Vector2 dir)
    {
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
            return dir.x > 0 ? Direction.Right : Direction.Left;
        else
            return dir.y > 0 ? Direction.Down : Direction.Up;
    }

    Vector2Int GetNeighborPosition(Vector2Int pos, Direction dir)
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
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (width <= 0 || height <= 0) return;

        float totalWidth = width * cellSize;
        float totalHeight = height * cellSize;

        Gizmos.color = gridColor;
        for (int x = 0; x <= width; x++)
        {
            Vector3 start = new Vector3(gridOffset.x + x * cellSize, gridOffset.y, 0);
            Vector3 end = new Vector3(gridOffset.x + x * cellSize, gridOffset.y + totalHeight, 0);
            Gizmos.DrawLine(start, end);
        }
        for (int y = 0; y <= height; y++)
        {
            Vector3 start = new Vector3(gridOffset.x, gridOffset.y + y * cellSize, 0);
            Vector3 end = new Vector3(gridOffset.x + totalWidth, gridOffset.y + y * cellSize, 0);
            Gizmos.DrawLine(start, end);
        }
    }
#endif
    Vector3 GetCellCenter(int x, int y)
    {
        int invertedY = height - 1 - y;
        return new Vector3(
            transform.position.x + x * cellSize + cellSize / 2 + gridOffset.x,
            transform.position.y + invertedY * cellSize + cellSize / 2 + gridOffset.y,
            0
        );
    }
    bool IsWithinGrid(Vector2Int pos) =>
        pos.x >= 0 && pos.x < width && pos.y >= 0 && pos.y < height;
}
public enum Direction { Up, Down, Left, Right }
