using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance { get; private set; }

    [Header("Grid Settings")]
    [SerializeField] private int width = 8;
    [SerializeField] private int height = 8;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Vector2 gridOffset = Vector2.zero;
    [SerializeField] private GameObject[] objectPrefabs;
    [SerializeField] private Color gridColor = Color.white;
    [SerializeField] private int initialSpawnCount = 2;

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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            MoveAllObjectsUp();
        }
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
        for (int i = 0; i < initialSpawnCount; i++)
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
        for (int y = height - 2; y >= 0; y--) // идём снизу вверх (чтобы не перезаписывать)
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
                            CheckMatches(toPos);
                            SpawnInitialObjects();
                        }
                    }
                }
            }
        }
    }

    public void SpawnRandomObject(int x, int y)
    {
        GameObject prefab = objectPrefabs[Random.Range(0, objectPrefabs.Length)];
        GameObject obj = Instantiate(prefab, GetCellCenter(x, y), Quaternion.identity);
        grid[x, y].Fill(obj);
    }

    public void OnBallHit(GameObject hitObject, Vector2 hitDirection)
    {
        Vector2Int fromPos = FindObjectPosition(hitObject);
        if (fromPos.x == -1) return;

        Direction dir = GetDirectionFromVector(hitDirection);
        if (MoveObject(fromPos, dir))
        {
            CheckMatches(fromPos);
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

        obj.transform.position = GetCellCenter(toPos.x, toPos.y);
        return true;
    }

    void CheckMatches(Vector2Int startPos)
    {
        var obj = grid[startPos.x, startPos.y].content;
        if (obj == null) return;

        string targetType = obj.GetComponent<GridObject>().Id;
        List<GridCell> matches = FindMatches(startPos, targetType);

        if (matches.Count >= 2) // Минимум 2 для матча
        {
            StartCoroutine(AnimateAndDestroyMatches(matches, 0.5f));
            print($"Match found! Count: {matches.Count}");
            SpawnInitialObjects();
        }
    }

    IEnumerator AnimateAndDestroyMatches(List<GridCell> matches, float duration)
    {
        // Сохраняем ссылки на объекты
        List<GameObject> objs = new List<GameObject>();
        foreach (var cell in matches)
        {
            if (cell.content != null)
            {
                objs.Add(cell.content);

                // ✅ Подсветка
                var renderer = cell.content.GetComponent<SpriteRenderer>();
                if (renderer != null)
                    renderer.color = Color.red;
            }
        }

        // ✅ Анимация уменьшения
        float elapsed = 0f;
        Vector3 startScale = Vector3.one;
        Vector3 endScale = Vector3.zero;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            foreach (var obj in objs)
            {
                if (obj != null)
                    obj.transform.localScale = Vector3.Lerp(startScale, endScale, t);
            }

            yield return null;
        }

        // ✅ Удаление объектов
        foreach (var cell in matches)
        {
            if (cell.content != null)
                Destroy(cell.content);
            cell.Clear();
        }

        // ✅ Спавн новых объектов
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

    IEnumerator RefillGrid()
    {
        yield return new WaitForSeconds(0.5f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (grid[x, y].IsEmpty)
                {
                    SpawnRandomObject(x, y);
                    yield return new WaitForSeconds(0.05f);
                }
            }
        }
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


