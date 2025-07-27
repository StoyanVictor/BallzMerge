using UnityEngine;

public class GridInitializer : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private int width = 5;
    [SerializeField] private int height = 7;
    [SerializeField] private float cellSize = 1.6f;
    [SerializeField] private Vector2 gridOffset = Vector2.zero;
    [SerializeField] private Color gridColor = Color.white;
    public GridCell[,] grid;

    public int GetWidth() => width;
    public int GetHeight() => height;
    public float GetCellSize() => cellSize;
    public Vector2 GetOffset() => gridOffset;
    public GridCell[,] GetGrid() => grid;
    void Awake() => InitializeGrid();
    
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
}